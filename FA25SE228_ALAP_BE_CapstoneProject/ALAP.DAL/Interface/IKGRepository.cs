using App.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface IKGRepository
    {
        Task<KGSubjectModel?> GetSubjectByIdAsync(long subjectId);
        Task<KGSubjectModel?> GetSubjectByCodeAsync(string subjectCode);
        Task<KGSubjectModel> CreateSubjectAsync(KGSubjectModel subject);
        Task UpdateSubjectAsync(KGSubjectModel subject);
        Task<KGSubjectModel?> GetSubjectBySubjectIdAsync(long subjectId);
        // Node
        Task<List<KGNodeModel>> GetNodesBySubjectIdAsync(long subjectId);
        Task<KGNodeModel?> GetNodeByIdAsync(long nodeId);
        Task BulkInsertNodesAsync(List<KGNodeModel> nodes);
        Task DeleteNodesBySubjectIdAsync(long subjectId);

        // Edge
        Task<List<KGEdgeModel>> GetEdgesBySubjectIdAsync(long subjectId);
        Task BulkInsertEdgesAsync(List<KGEdgeModel> edges);
        Task DeleteEdgesBySubjectIdAsync(long subjectId);
    }
}
