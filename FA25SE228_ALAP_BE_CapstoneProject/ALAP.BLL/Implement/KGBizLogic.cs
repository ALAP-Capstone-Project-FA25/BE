// App.BizLogic/Implement/KGBizLogic.cs
using ALAP.BLL;
using ALAP.BLL.Interface;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.KnowledgeGraph;
using ALAP.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ALAP.BizLogic.Implement
{
    public class KGBizLogic : AppBaseBizLogic, IKGBizLogic
    {
        private readonly IKGRepository _repository;
        private readonly ILogger<KGBizLogic> _logger;
        private readonly BaseDBContext _dbContext;
        public KGBizLogic(BaseDBContext dbContext, IKGRepository repository, ILogger<KGBizLogic> logger) : base(dbContext)
        {
            _dbContext = dbContext;
            _repository = repository;
            _logger = logger;
        }


        public async Task<KGImportResultDto> ImportKnowledgeGraphAsync(long subjectId, KGImportDto dto)
        {
            var result = new KGImportResultDto { SubjectId = subjectId };

            try
            {
                var subject = await _repository.GetSubjectBySubjectIdAsync(subjectId);
                if (subject == null)
                {
                    var existingSubject = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == subjectId) ?? throw new Exception ("Not found subject");
                    subject = new KGSubjectModel
                    {
                        SubjectCode = existingSubject.Name,
                        SubjectId = existingSubject.Id,
                        SubjectName = existingSubject.Name,
                        Version = dto.Version,
                        ExportedAt = dto.ExportedAt
                    };
                    subject = await _repository.CreateSubjectAsync(subject);
                }

                await ProcessImportAsync(subject, dto, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi import knowledge graph cho subject {SubjectId}", subjectId);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public async Task<KGImportResultDto> ImportKnowledgeGraphAsync(string subjectCode, KGImportDto dto)
        {
            var result = new KGImportResultDto();

            try
            {
                var subject = await _repository.GetSubjectByCodeAsync(subjectCode);
                if (subject == null)
                {
                    subject = new KGSubjectModel
                    {
                        SubjectCode = subjectCode,
                        SubjectName = subjectCode,
                        Version = dto.Version,
                        ExportedAt = dto.ExportedAt
                    };
                    subject = await _repository.CreateSubjectAsync(subject);
                }

                result.SubjectId = subject.Id;
                await ProcessImportAsync(subject, dto, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi import knowledge graph cho subject code {SubjectCode}", subjectCode);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task ProcessImportAsync(KGSubjectModel subject, KGImportDto dto, KGImportResultDto result)
        {
            subject.Version = dto.Version;
            subject.ExportedAt = dto.ExportedAt;
            await _repository.UpdateSubjectAsync(subject);

            await _repository.DeleteEdgesBySubjectIdAsync(subject.Id);
            await _repository.DeleteNodesBySubjectIdAsync(subject.Id);

            var nodesToInsert = new List<KGNodeModel>();

            foreach (var nodeDto in dto.Nodes)
            {
                if (string.IsNullOrWhiteSpace(nodeDto.Id))
                {
                    result.NodesSkipped++;
                    result.Warnings.Add("Bỏ qua node có ID rỗng");
                    continue;
                }

                var node = MapToNodeModel(subject.Id, nodeDto);
                nodesToInsert.Add(node);
            }

            await _repository.BulkInsertNodesAsync(nodesToInsert);
            result.NodesImported = nodesToInsert.Count;

            // 4. Build mapping nodeCode -> nodeId
            var insertedNodes = await _repository.GetNodesBySubjectIdAsync(subject.Id);
            var nodeCodeToIdMap = insertedNodes.ToDictionary(n => n.NodeCode, n => n.Id);

            // 5. Insert edges
            var edgesToInsert = new List<KGEdgeModel>();

            foreach (var edgeDto in dto.Edges)
            {
                if (string.IsNullOrWhiteSpace(edgeDto.Source) || string.IsNullOrWhiteSpace(edgeDto.Target))
                {
                    result.EdgesSkipped++;
                    result.Warnings.Add($"Bỏ qua edge {edgeDto.Id}: thiếu source hoặc target");
                    continue;
                }

                if (!nodeCodeToIdMap.TryGetValue(edgeDto.Source, out var sourceId))
                {
                    result.EdgesSkipped++;
                    result.Warnings.Add($"Bỏ qua edge {edgeDto.Id}: không tìm thấy source node '{edgeDto.Source}'");
                    continue;
                }

                if (!nodeCodeToIdMap.TryGetValue(edgeDto.Target, out var targetId))
                {
                    result.EdgesSkipped++;
                    result.Warnings.Add($"Bỏ qua edge {edgeDto.Id}: không tìm thấy target node '{edgeDto.Target}'");
                    continue;
                }

                var edge = new KGEdgeModel
                {
                    SubjectId = subject.Id,
                    EdgeCode = edgeDto.Id,
                    SourceNodeId = sourceId,
                    TargetNodeId = targetId,
                    EdgeType = edgeDto.Type ?? "required"
                };

                edgesToInsert.Add(edge);
            }

            await _repository.BulkInsertEdgesAsync(edgesToInsert);
            result.EdgesImported = edgesToInsert.Count;
            result.Success = true;

            _logger.LogInformation(
                "Import thành công subject {SubjectId}: {NodesCount} nodes, {EdgesCount} edges",
                subject.Id, result.NodesImported, result.EdgesImported);
        }

        private static KGNodeModel MapToNodeModel(long subjectId, KGNodeImportDto dto)
        {
            return new KGNodeModel
            {
                SubjectId = subjectId,
                NodeCode = dto.Id,
                PositionX = dto.Position.X,
                PositionY = dto.Position.Y,
                Label = dto.Data.Label ?? string.Empty,
                Category = dto.Data.Category ?? string.Empty,
                Description = dto.Data.Description,
                EstimatedTime = dto.Data.EstimatedTime,
                Difficulty = dto.Data.Difficulty ?? "Cơ bản",
                Status = dto.Data.Status ?? "locked",
                ConceptsJson = JsonSerializer.Serialize(dto.Data.Concepts ?? []),
                ExamplesJson = JsonSerializer.Serialize(dto.Data.Examples ?? []),
                PrerequisitesJson = JsonSerializer.Serialize(dto.Data.Prerequisites ?? []),
                ResourcesJson = JsonSerializer.Serialize(dto.Data.Resources ?? [])
            };
        }

        public async Task<KGExportDto> ExportKnowledgeGraphAsync(long subjectId)
        {
            var subject = await _repository.GetSubjectBySubjectIdAsync(subjectId);

            if (subject == null)
            {
                var existingSubject = await _dbContext.Categories
                    .FirstOrDefaultAsync(x => x.Id == subjectId);

                if (existingSubject == null)
                {
                    return new KGExportDto
                    {
                        Version = "1.0",
                        ExportedAt = DateTime.UtcNow,
                        Nodes = new List<KGNodeImportDto>(),
                        Edges = new List<KGEdgeImportDto>()
                    };
                }

                subject = new KGSubjectModel
                {
                    SubjectCode = existingSubject.Name,
                    SubjectId = existingSubject.Id,
                    SubjectName = existingSubject.Name,
                    Version = "1.0",           
                    ExportedAt = DateTime.UtcNow
                };

                subject = await _repository.CreateSubjectAsync(subject);

                var rootNode = new KGNodeModel
                {
                    SubjectId = subject.Id,
                    NodeCode = "root",        
                    PositionX = 0,
                    PositionY = 0,
                    Label = existingSubject.Name,
                    Category = string.Empty,
                    Description = string.Empty,
                    EstimatedTime = string.Empty,
                    Difficulty = "Cơ bản",
                    Status = "unlocked",
                    ConceptsJson = JsonSerializer.Serialize(new List<string>()),
                    ExamplesJson = JsonSerializer.Serialize(new List<string>()),
                    PrerequisitesJson = JsonSerializer.Serialize(new List<string>()),
                    ResourcesJson = JsonSerializer.Serialize(new List<KGResourceDto>())
                };

                await _repository.BulkInsertNodesAsync(new List<KGNodeModel> { rootNode });

                return new KGExportDto
                {
                    Version = subject.Version,
                    ExportedAt = subject.CreatedAt,
                    Nodes = new List<KGNodeImportDto>
            {
                new KGNodeImportDto
                {
                    Id = rootNode.NodeCode,
                    Position = new KGPositionDto
                    {
                        X = rootNode.PositionX,
                        Y = rootNode.PositionY
                    },
                    Data = new KGNodeDataDto
                    {
                        Label = rootNode.Label,
                        Category = rootNode.Category,
                        Description = rootNode.Description ?? string.Empty,
                        Concepts = new List<string>(),
                        Examples = new List<string>(),
                        Prerequisites = new List<string>(),
                        EstimatedTime = rootNode.EstimatedTime ?? string.Empty,
                        Difficulty = rootNode.Difficulty,
                        Status = rootNode.Status,
                        Resources = new List<KGResourceDto>()
                    }
                }
            },
                    Edges = new List<KGEdgeImportDto>()
                };
            }

            var nodes = await _repository.GetNodesBySubjectIdAsync(subject.Id);
            var edges = await _repository.GetEdgesBySubjectIdAsync(subject.Id);

            var nodeIdToCodeMap = nodes.ToDictionary(n => n.Id, n => n.NodeCode);

            return new KGExportDto
            {
                Version = subject.Version,
                ExportedAt = DateTime.UtcNow,
                Nodes = nodes.Select(n => new KGNodeImportDto
                {
                    Id = n.NodeCode,
                    Position = new KGPositionDto { X = n.PositionX, Y = n.PositionY },
                    Data = new KGNodeDataDto
                    {
                        Label = n.Label,
                        Category = n.Category,
                        Description = n.Description ?? string.Empty,
                        Concepts = JsonSerializer.Deserialize<List<string>>(n.ConceptsJson) ?? [],
                        Examples = JsonSerializer.Deserialize<List<string>>(n.ExamplesJson) ?? [],
                        Prerequisites = JsonSerializer.Deserialize<List<string>>(n.PrerequisitesJson) ?? [],
                        EstimatedTime = n.EstimatedTime ?? string.Empty,
                        Difficulty = n.Difficulty,
                        Status = n.Status,
                        Resources = JsonSerializer.Deserialize<List<KGResourceDto>>(n.ResourcesJson) ?? []
                    }
                }).ToList(),
                Edges = edges.Select(e => new KGEdgeImportDto
                {
                    Id = e.EdgeCode,
                    Source = nodeIdToCodeMap.GetValueOrDefault(e.SourceNodeId, string.Empty),
                    Target = nodeIdToCodeMap.GetValueOrDefault(e.TargetNodeId, string.Empty),
                    Type = e.EdgeType
                }).ToList()
            };
        }


        public async Task<KGSubjectModel?> GetSubjectAsync(long subjectId)
        {
            return await _repository.GetSubjectByIdAsync(subjectId);
        }

        public async Task<List<KGNodeModel>> GetNodesAsync(long subjectId)
        {
            return await _repository.GetNodesBySubjectIdAsync(subjectId);
        }

        public async Task<List<KGEdgeModel>> GetEdgesAsync(long subjectId)
        {
            return await _repository.GetEdgesBySubjectIdAsync(subjectId);
        }
    }
}