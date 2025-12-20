using ALAP.Entity.DTO.Response;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IKnowledgeGraphBizLogic
    {
        Task<PersonalizedRoadmapDto> GetPersonalizedRoadmap(long subjectId, long userId);
        Task<bool> UpdateNodeProgress(long userId, long nodeId, int progressPercent);
        Task<bool> MarkNodeAsCompleted(long userId, long nodeId);
        Task<bool> StartNode(long userId, long nodeId);
        Task<bool> ImportKnowledgeGraph(App.Entity.DTO.Request.ImportKnowledgeGraphDto dto);
    }
}
