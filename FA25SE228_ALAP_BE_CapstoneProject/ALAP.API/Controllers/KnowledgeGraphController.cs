using ALAP.BLL.Interface;
using ALAP.Entity.DTO.KnowledgeGraph;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowledgeGraphController : BaseAPIController
    {
        private readonly IKnowledgeGraphBizLogic _knowledgeGraphBiz;
        private readonly IKGBizLogic _kGBizLogic;
        private readonly ILogger<KnowledgeGraphController> _logger;

        public KnowledgeGraphController(
            IKnowledgeGraphBizLogic knowledgeGraphBiz,
            ILogger<KnowledgeGraphController> logger,
            IKGBizLogic kGBizLogic)
        {
            _knowledgeGraphBiz = knowledgeGraphBiz;
            _logger = logger;
            _kGBizLogic = kGBizLogic;
        }

        [HttpGet("subjects/{subjectId}/personalized")]
        [Authorize]
        public async Task<IActionResult> GetPersonalizedRoadmap(long subjectId)
        {
            try
            {
                var result = await _knowledgeGraphBiz.GetPersonalizedRoadmap(subjectId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetPersonalizedRoadmap] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        [HttpPost("nodes/{nodeId}/start")]
        [Authorize]
        public async Task<IActionResult> StartNode(long nodeId)
        {
            try
            {
                var result = await _knowledgeGraphBiz.StartNode(UserId, nodeId);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[StartNode] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpPut("nodes/{nodeId}/progress")]
        [Authorize]
        public async Task<IActionResult> UpdateNodeProgress(long nodeId, [FromBody] int progressPercent)
        {
            try
            {
                var result = await _knowledgeGraphBiz.UpdateNodeProgress(UserId, nodeId, progressPercent);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[UpdateNodeProgress] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpPost("nodes/{nodeId}/complete")]
        [Authorize]
        public async Task<IActionResult> MarkNodeAsCompleted(long nodeId)
        {
            try
            {
                var result = await _knowledgeGraphBiz.MarkNodeAsCompleted(UserId, nodeId);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[MarkNodeAsCompleted] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpGet("subjects/{subjectId}/export")]
        [AllowAnonymous]
        public async Task<IActionResult> ExportRoadmap(long subjectId)
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var result = await _knowledgeGraphBiz.GetPersonalizedRoadmap(subjectId, UserId);
                    return GetSuccess(result);
                }

                var guestResult = await _knowledgeGraphBiz.GetPersonalizedRoadmap(subjectId, 0);
                return GetSuccess(guestResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ExportRoadmap] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        [HttpPost("subjects/{subjectId:long}/import")]
        public async Task<IActionResult> ImportBySubjectId(long subjectId, [FromBody] KGImportDto dto)
        {

            try
            {
                if (dto == null || dto.Nodes.Count == 0)
                {
                    return BadRequest(new { message = "Payload không hợp lệ: cần có nodes" });
                }

                var result = await _kGBizLogic.ImportKnowledgeGraphAsync(subjectId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex);
            }
        }
    }
}
