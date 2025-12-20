using ALAP.Entity.DTO.KnowledgeGraph;
using ALAP.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IKGBizLogic
    {
        Task<KGImportResultDto> ImportKnowledgeGraphAsync(long subjectId, KGImportDto dto);
        Task<KGImportResultDto> ImportKnowledgeGraphAsync(string subjectCode, KGImportDto dto);
        Task<KGExportDto> ExportKnowledgeGraphAsync(long subjectId);
        Task<KGSubjectModel?> GetSubjectAsync(long subjectId);
        Task<List<KGNodeModel>> GetNodesAsync(long subjectId);
        Task<List<KGEdgeModel>> GetEdgesAsync(long subjectId);
    }
}
