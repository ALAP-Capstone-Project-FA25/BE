using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implement
{
    public class KGRepository : IKGRepository
    {
        private readonly BaseDBContext _context;

        public KGRepository(BaseDBContext context)
        {
            _context = context;
        }

        #region Subject

        public async Task<KGSubjectModel?> GetSubjectByIdAsync(long subjectId)
        {
            return await _context.KGSubjects
                .FirstOrDefaultAsync(x => x.Id == subjectId && x.IsActive);
        }

        public async Task<KGSubjectModel?> GetSubjectBySubjectIdAsync(long subjectId)
        {
            return await _context.KGSubjects
                .FirstOrDefaultAsync(x => x.SubjectId == subjectId && x.IsActive);
        }

        public async Task<KGSubjectModel?> GetSubjectByCodeAsync(string subjectCode)
        {
            return await _context.KGSubjects
                .FirstOrDefaultAsync(x => x.SubjectCode == subjectCode && x.IsActive);
        }

        public async Task<KGSubjectModel> CreateSubjectAsync(KGSubjectModel subject)
        {
            subject.CreatedAt = DateTime.UtcNow;
            await _context.KGSubjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task UpdateSubjectAsync(KGSubjectModel subject)
        {
            subject.UpdatedAt = DateTime.UtcNow;
            _context.KGSubjects.Update(subject);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Node

        public async Task<List<KGNodeModel>> GetNodesBySubjectIdAsync(long subjectId)
        {
            return await _context.KGNodes
                .Where(x => x.SubjectId == subjectId)
                .ToListAsync();
        }

        public async Task<KGNodeModel?> GetNodeByIdAsync(long nodeId)
        {
            return await _context.KGNodes
                .Include(x => x.OutgoingEdges)
                .Include(x => x.IncomingEdges)
                .FirstOrDefaultAsync(x => x.Id == nodeId);
        }

        public async Task BulkInsertNodesAsync(List<KGNodeModel> nodes)
        {
            if (nodes.Count == 0) return;

            var now = DateTime.UtcNow;
            foreach (var node in nodes)
            {
                node.CreatedAt = now;
            }

            await _context.KGNodes.AddRangeAsync(nodes);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNodesBySubjectIdAsync(long subjectId)
        {
            await _context.KGNodes
                .Where(x => x.SubjectId == subjectId)
                .ExecuteDeleteAsync();
        }

        #endregion

        #region Edge

        public async Task<List<KGEdgeModel>> GetEdgesBySubjectIdAsync(long subjectId)
        {
            return await _context.KGEdges
                .Where(x => x.SubjectId == subjectId)
                .ToListAsync();
        }

        public async Task BulkInsertEdgesAsync(List<KGEdgeModel> edges)
        {
            if (edges.Count == 0) return;

            var now = DateTime.UtcNow;
            foreach (var edge in edges)
            {
                edge.CreatedAt = now;
            }

            await _context.KGEdges.AddRangeAsync(edges);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEdgesBySubjectIdAsync(long subjectId)
        {
            await _context.KGEdges
                .Where(x => x.SubjectId == subjectId)
                .ExecuteDeleteAsync();
        }

        #endregion
    }
}
