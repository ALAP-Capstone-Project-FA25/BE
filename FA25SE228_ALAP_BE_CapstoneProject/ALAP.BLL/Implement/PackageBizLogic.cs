using ALAP.BLL.Helper;
using ALAP.BLL.Interface;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using ALAP.Entity.Models.Enums;

namespace ALAP.BLL.Implement
{
    public class PackageBizLogic : AppBaseBizLogic, IPackageBizLogic
    {
        private readonly IPackageRepository _packageRepository;
        private readonly PayOsClient _payos;

        public PackageBizLogic(BaseDBContext dbContext, IPackageRepository packageBizLogic, PayOsClient payos) : base(dbContext)
        {
            _packageRepository = packageBizLogic;
            _payos = payos;
        }

        public async Task<bool> CreateUpdatePackage(CreateUpdatePackageDto dto)
        {
            if (dto.Id > 0)
            {
                var model = new PackageModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    PackageType = dto.PackageType,
                    Price = dto.Price,
                    Duration = dto.Duration,
                    IsActive = dto.IsActive,
                    UpdatedAt = Utils.GetCurrentVNTime(),
                };
                return await _packageRepository.Update(model);
            }
            else
            {
                var model = new PackageModel
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    PackageType = dto.PackageType,
                    Price = dto.Price,
                    Duration = dto.Duration,
                    IsActive = dto.IsActive,
                };
                return await _packageRepository.Create(model);
            }
        }

        public async Task<bool> DeletePackage(long id)
        {
            return await _packageRepository.Delete(id);
        }

        public async Task<PackageModel> GetPackageById(long id)
        {
            return await _packageRepository.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy gói học.");
        }

        public async Task<PagedResult<PackageModel>> GetListPackagesByPaging(PagingModel pagingModel)
        {
            return await _packageRepository.GetListByPaging(pagingModel);
        }

        public async Task<string> BuyPackage(long packageId, long userId)
        {
            var existingPackage = await _packageRepository.GetById(packageId);
            if (existingPackage == null)
            {
                throw new KeyNotFoundException("Gói học không tồn tại.");
            }

            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var dto = new CreatePaymentRequest
            {
                Amount = existingPackage.Price,
                Description = "Thanh toan goi",
                OrderCode = orderCode,
                CancelUrl = "https://alap.com/payment/cancel",
                ReturnUrl = "https://alap.com/payment/success",
            };

            var res = await _payos.CreatePaymentAsync(dto);
            var item = Utils.SerializeObjectToJson(existingPackage);
            var paymentModel = new PaymentModel
            {
                UserId = userId,
                PackageId = packageId,
                Item = item,
                Amount = existingPackage.Price,
                Code = orderCode,
                PaymentUrl = res.Data?.CheckoutUrl ?? "",
                PaymentStatus = PaymentStatus.PENDING,
                PaymentType = PaymentType.PACKAGE,
                QrCode = res.Data?.QrCode ?? "",
                CreatedAt = Utils.GetCurrentVNTime(),
            };
            await _dbContext.Payments.AddAsync(paymentModel);
            await _dbContext.SaveChangesAsync();

            return res.Data?.CheckoutUrl ?? "";


        }
    }
}


