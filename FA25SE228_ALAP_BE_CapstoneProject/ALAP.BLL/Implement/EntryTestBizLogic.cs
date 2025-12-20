using ALAP.BLL.Interface;
using ALAP.DAL.DataBase;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class EntryTestBizLogic : AppBaseBizLogic, IEntryTestBizLogic
    {
        private readonly IEntryTestRepository _entryTestRepository;
        private readonly BaseDBContext _dbContext;

        public EntryTestBizLogic(BaseDBContext dbContext, IEntryTestRepository entryTestRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            _entryTestRepository = entryTestRepository;
        }

        public async Task<bool> CreateUpdateEntryTest(CreateUpdateEntryTestDto dto)
        {
            if (dto.Id > 0)
            {
                // Update
                var existing = await _entryTestRepository.GetByIdWithDetails(dto.Id);
                if (existing == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy bài test.");
                }

                existing.Title = dto.Title;
                existing.Description = dto.Description;
                existing.IsActive = dto.IsActive;
                existing.DisplayOrder = dto.DisplayOrder;
                existing.UpdatedAt = Utils.GetCurrentVNTime();

                // Remove old questions
                var oldQuestions = await _dbContext.EntryTestQuestions
                    .Include(q => q.Options)
                        .ThenInclude(o => o.SubjectMappings)
                    .Where(q => q.EntryTestId == dto.Id)
                    .ToListAsync();

                foreach (var q in oldQuestions)
                {
                    foreach (var o in q.Options)
                    {
                        _dbContext.EntryTestSubjectMappings.RemoveRange(o.SubjectMappings);
                    }
                    _dbContext.EntryTestOptions.RemoveRange(q.Options);
                }
                _dbContext.EntryTestQuestions.RemoveRange(oldQuestions);

                // Add new questions
                foreach (var qDto in dto.Questions)
                {
                    var question = new EntryTestQuestionModel
                    {
                        EntryTestId = existing.Id,
                        QuestionText = qDto.QuestionText,
                        DisplayOrder = qDto.DisplayOrder
                    };
                    await _dbContext.EntryTestQuestions.AddAsync(question);
                    await _dbContext.SaveChangesAsync();

                    foreach (var oDto in qDto.Options)
                    {
                        var option = new EntryTestOptionModel
                        {
                            QuestionId = question.Id,
                            OptionCode = oDto.OptionCode,
                            OptionText = oDto.OptionText,
                            DisplayOrder = oDto.DisplayOrder
                        };
                        await _dbContext.EntryTestOptions.AddAsync(option);
                        await _dbContext.SaveChangesAsync();

                        foreach (var categoryId in oDto.CategoryIds)
                        {
                            var mapping = new EntryTestSubjectMappingModel
                            {
                                OptionId = option.Id,
                                CategoryId = categoryId,
                                Weight = oDto.Weight
                            };
                            await _dbContext.EntryTestSubjectMappings.AddAsync(mapping);
                        }
                    }
                }

                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            else
            {
                // Create
                var model = new EntryTestModel
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                    DisplayOrder = dto.DisplayOrder
                };
                await _dbContext.EntryTests.AddAsync(model);
                await _dbContext.SaveChangesAsync();

                foreach (var qDto in dto.Questions)
                {
                    var question = new EntryTestQuestionModel
                    {
                        EntryTestId = model.Id,
                        QuestionText = qDto.QuestionText,
                        DisplayOrder = qDto.DisplayOrder
                    };
                    await _dbContext.EntryTestQuestions.AddAsync(question);
                    await _dbContext.SaveChangesAsync();

                    foreach (var oDto in qDto.Options)
                    {
                        var option = new EntryTestOptionModel
                        {
                            QuestionId = question.Id,
                            OptionCode = oDto.OptionCode,
                            OptionText = oDto.OptionText,
                            DisplayOrder = oDto.DisplayOrder
                        };
                        await _dbContext.EntryTestOptions.AddAsync(option);
                        await _dbContext.SaveChangesAsync();

                        foreach (var categoryId in oDto.CategoryIds)
                        {
                            var mapping = new EntryTestSubjectMappingModel
                            {
                                OptionId = option.Id,
                                CategoryId = categoryId,
                                Weight = oDto.Weight
                            };
                            await _dbContext.EntryTestSubjectMappings.AddAsync(mapping);
                        }
                    }
                }

                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
        }

        public async Task<bool> DeleteEntryTest(long id)
        {
            return await _entryTestRepository.Delete(id);
        }

        public async Task<List<EntryTestModel>> GetAllActiveEntryTests()
        {
            var tests = await _dbContext.EntryTests
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
            return tests;
        }

        public async Task<EntryTestResultModel> GetUserTestResult(long userId, long entryTestId)
        {
            return await _dbContext.EntryTestResults
                .Where(x => x.UserId == userId && x.EntryTestId == entryTestId)
                .OrderByDescending(x => x.CompletedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<EntryTestModel> GetEntryTestById(long id)
        {
            return await _entryTestRepository.GetByIdWithDetails(id) 
                ?? throw new KeyNotFoundException("Không tìm thấy bài test.");
        }

        public async Task<PagedResult<EntryTestModel>> GetListEntryTestsByPaging(PagingModel pagingModel)
        {
            return await _entryTestRepository.GetListByPaging(pagingModel);
        }

        public async Task<EntryTestResultDto> SubmitEntryTest(long userId, SubmitEntryTestDto dto)
        {
            var test = await _entryTestRepository.GetByIdWithDetails(dto.EntryTestId);
            if (test == null)
            {
                throw new KeyNotFoundException("Không tìm thấy bài test.");
            }

            // Calculate scores
            var categoryScores = new Dictionary<long, int>();
            
            foreach (var answer in dto.Answers)
            {
                var questionId = answer.Key;
                var optionCode = answer.Value;

                var question = test.Questions.FirstOrDefault(q => q.Id == questionId);
                if (question == null) continue;

                var option = question.Options.FirstOrDefault(o => o.OptionCode == optionCode);
                if (option == null) continue;

                foreach (var mapping in option.SubjectMappings)
                {
                    if (!categoryScores.ContainsKey(mapping.CategoryId))
                    {
                        categoryScores[mapping.CategoryId] = 0;
                    }
                    categoryScores[mapping.CategoryId] += mapping.Weight;
                }
            }

            // Get top subjects
            var topSubjects = categoryScores
                .OrderByDescending(x => x.Value)
                .Take(5)
                .ToList();

            var recommendedSubjects = new List<SubjectRecommendationDto>();
            foreach (var (categoryId, score) in topSubjects)
            {
                var category = await _dbContext.Categories
                    .FirstOrDefaultAsync(c => c.Id == categoryId);
                if (category != null)
                {
                    recommendedSubjects.Add(new SubjectRecommendationDto
                    {
                        CategoryId = categoryId,
                        CategoryName = category.Name,
                        Description = category.Description,
                        ImageUrl = category.ImageUrl,
                        Score = score
                    });
                }
            }

            // Calculate major scores based on subjects
            var majorScores = new Dictionary<long, int>();
            foreach (var (categoryId, score) in categoryScores)
            {
                var category = await _dbContext.Categories
                    .Include(c => c.Major)
                    .FirstOrDefaultAsync(c => c.Id == categoryId);
                
                if (category?.Major != null)
                {
                    if (!majorScores.ContainsKey(category.MajorId))
                    {
                        majorScores[category.MajorId] = 0;
                    }
                    majorScores[category.MajorId] += score;
                }
            }

            // Get top majors
            var topMajors = majorScores
                .OrderByDescending(x => x.Value)
                .Take(3)
                .ToList();

            var recommendedMajors = new List<MajorRecommendationDto>();
            foreach (var (majorId, score) in topMajors)
            {
                var major = await _dbContext.Majors
                    .Include(m => m.Categories)
                    .FirstOrDefaultAsync(m => m.Id == majorId);
                
                if (major != null)
                {
                    recommendedMajors.Add(new MajorRecommendationDto
                    {
                        MajorId = majorId,
                        MajorName = major.Name,
                        Description = major.Description,
                        Score = score,
                        RelatedSubjects = major.Categories.Select(c => c.Name).ToList()
                    });
                }
            }

            // Save result
            var result = new EntryTestResultModel
            {
                UserId = userId,
                EntryTestId = dto.EntryTestId,
                AnswersJson = JsonSerializer.Serialize(dto.Answers),
                RecommendedSubjectsJson = JsonSerializer.Serialize(recommendedSubjects),
                RecommendedMajorsJson = JsonSerializer.Serialize(recommendedMajors),
                CompletedAt = Utils.GetCurrentVNTime()
            };
            await _dbContext.EntryTestResults.AddAsync(result);
            await _dbContext.SaveChangesAsync();

            return new EntryTestResultDto
            {
                RecommendedSubjects = recommendedSubjects,
                RecommendedMajors = recommendedMajors
            };
        }
    }
}
