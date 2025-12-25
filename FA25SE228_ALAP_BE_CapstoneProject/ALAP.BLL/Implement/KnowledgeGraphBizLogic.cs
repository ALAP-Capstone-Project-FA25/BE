using ALAP.BLL.Interface;
using ALAP.DAL.Database;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class KnowledgeGraphBizLogic : AppBaseBizLogic, IKnowledgeGraphBizLogic
    {
        public KnowledgeGraphBizLogic(BaseDBContext dbContext) : base(dbContext)
        {
        }

        public async Task<PersonalizedRoadmapDto> GetPersonalizedRoadmap(long subjectId, long userId)
        {
            var subject = await _dbContext.KGSubjects
                .Include(s => s.Nodes)
                .Include(s => s.Edges)
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(s => s.SubjectId == subjectId && s.IsActive);

            if (subject == null)
            {
                subject = await CreateDefaultKnowledgeGraph(subjectId);
            }

            HashSet<long> purchasedCourseIds;
            HashSet<long> completedCourseIds;

            if (userId > 0)
            {
                var userCourses = await _dbContext.UserCourses
                    .Where(uc => uc.UserId == userId && uc.IsActive)
                    .Select(uc => new { uc.CourseId, uc.IsDone })
                    .ToListAsync();

                purchasedCourseIds = userCourses.Select(uc => uc.CourseId).ToHashSet();
                completedCourseIds = userCourses.Where(uc => uc.IsDone).Select(uc => uc.CourseId).ToHashSet();
            }
            else
            {
                purchasedCourseIds = new HashSet<long>();
                completedCourseIds = new HashSet<long>();
            }

            var userProgress = userId > 0
                ? await _dbContext.Set<UserKnowledgeProgressModel>()
                    .Where(up => up.UserId == userId && up.SubjectId == subjectId)
                    .ToDictionaryAsync(up => up.NodeId, up => up)
                : new Dictionary<long, UserKnowledgeProgressModel>();

            var nodes = new List<RoadmapNodeDto>();
            var nodeStatusMap = new Dictionary<string, string>();

            var sortedNodes = TopologicalSort(subject.Nodes);

            foreach (var node in sortedNodes)
            {
                var nodeCode = node.NodeCode;
                var progress = userProgress.ContainsKey(node.Id) ? userProgress[node.Id] : null;

                var concepts = ParseJsonArray(node.ConceptsJson);
                var examples = ParseJsonArray(node.ExamplesJson);
                var prerequisites = ParseJsonArray(node.PrerequisitesJson);
                var resources = ParseResources(node.ResourcesJson, purchasedCourseIds);

                var status = CalculateNodeStatus(node, prerequisites, nodeStatusMap, progress, resources, completedCourseIds, purchasedCourseIds);
                nodeStatusMap[nodeCode] = status;

                var relatedCourseIds = resources
                    .Where(r => r.CourseId.HasValue)
                    .Select(r => r.CourseId.Value)
                    .ToList();

                var hasPurchasedCourse = relatedCourseIds.Any(cid => purchasedCourseIds.Contains(cid));

                nodes.Add(new RoadmapNodeDto
                {
                    Id = nodeCode,
                    Type = "knowledge",
                    Position = new PositionDto
                    {
                        X = node.PositionX,
                        Y = node.PositionY
                    },
                    Data = new NodeDataDto
                    {
                        Label = node.Label,
                        Category = node.Category,
                        Description = node.Description ?? "",
                        Concepts = concepts,
                        Examples = examples,
                        Prerequisites = prerequisites,
                        EstimatedTime = node.EstimatedTime ?? "1-2 tuần",
                        Difficulty = node.Difficulty,
                        Status = status,
                        Resources = resources,
                        ProgressPercent = progress?.ProgressPercent ?? 0,
                        HasPurchasedCourse = hasPurchasedCourse,
                        RelatedCourseIds = relatedCourseIds
                    }
                });
            }

            var nodeIdToCodeMap = subject.Nodes.ToDictionary(n => n.Id, n => n.NodeCode);

            var edges = subject.Edges
                .Where(e => nodeIdToCodeMap.ContainsKey(e.SourceNodeId) && nodeIdToCodeMap.ContainsKey(e.TargetNodeId))
                .Select(e => new RoadmapEdgeDto
                {
                    Id = $"e{e.Id}",
                    Source = nodeIdToCodeMap[e.SourceNodeId],
                    Target = nodeIdToCodeMap[e.TargetNodeId],
                    Type = e.EdgeType ?? "required"
                }).ToList();

            var stats = CalculateStatistics(nodes, purchasedCourseIds.Count, completedCourseIds.Count);

            return new PersonalizedRoadmapDto
            {
                SubjectId = subjectId,
                SubjectName = subject.SubjectName,
                Description = subject.Description ?? "",
                Nodes = nodes,
                Edges = edges,
                Statistics = stats
            };
        }

        private List<KGNodeModel> TopologicalSort(ICollection<KGNodeModel> nodes)
        {
            var nodeDict = nodes.ToDictionary(n => n.NodeCode, n => n);
            var visited = new HashSet<string>();
            var result = new List<KGNodeModel>();

            void Visit(string nodeCode)
            {
                if (visited.Contains(nodeCode)) return;
                if (!nodeDict.ContainsKey(nodeCode)) return;

                visited.Add(nodeCode);

                var node = nodeDict[nodeCode];
                var prerequisites = ParseJsonArray(node.PrerequisitesJson);

                // Visit prerequisites first
                foreach (var prereq in prerequisites)
                {
                    Visit(prereq);
                }

                result.Add(node);
            }

            // Visit all nodes
            foreach (var node in nodes)
            {
                Visit(node.NodeCode);
            }

            return result;
        }

        private async Task<KGSubjectModel> CreateDefaultKnowledgeGraph(long subjectId)
        {
            // Lấy thông tin subject
            var subjectInfo = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == subjectId);

            var kgSubject = new KGSubjectModel
            {
                SubjectId = subjectId,
                SubjectCode = subjectInfo?.Name ?? $"SUBJECT_{subjectId}",
                SubjectName = subjectInfo?.Name ?? "Môn học",
                Description = "Lộ trình học tập mặc định",
                Version = "1.0",
                IsActive = true,
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.KGSubjects.AddAsync(kgSubject);
            await _dbContext.SaveChangesAsync();

            // Tạo 1 node mặc định
            var defaultNode = new KGNodeModel
            {
                SubjectId = kgSubject.Id,
                NodeCode = "node_default_1",
                PositionX = 250,
                PositionY = 100,
                Label = "Bắt đầu học",
                Category = "foundation",
                Description = "Điểm khởi đầu cho lộ trình học tập",
                EstimatedTime = "1-2 tuần",
                Difficulty = "Cơ bản",
                Status = "available",
                ConceptsJson = JsonSerializer.Serialize(new List<string> { "Giới thiệu môn học" }),
                ExamplesJson = JsonSerializer.Serialize(new List<string>()),
                PrerequisitesJson = JsonSerializer.Serialize(new List<string>()),
                ResourcesJson = JsonSerializer.Serialize(new List<object>()),
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.KGNodes.AddAsync(defaultNode);
            await _dbContext.SaveChangesAsync();

            // Load lại subject với đầy đủ relationships
            return await _dbContext.KGSubjects
                .Include(s => s.Nodes)
                .Include(s => s.Edges)
                .Include(s => s.Subject)
                .FirstAsync(s => s.Id == kgSubject.Id);
        }

        private string CalculateNodeStatus(
            KGNodeModel node,
            List<string> prerequisites,
            Dictionary<string, string> nodeStatusMap,
            UserKnowledgeProgressModel progress,
            List<ResourceDto> resources,
            HashSet<long> completedCourseIds,
            HashSet<long> purchasedCourseIds)
        {
            if (progress != null && progress.Status == "completed")
            {
                return "completed";
            }

            if (progress != null && progress.Status == "in-progress")
            {
                return "in-progress";
            }

            var relatedCourseIds = resources
                .Where(r => r.CourseId.HasValue)
                .Select(r => r.CourseId.Value)
                .ToList();

            // Nếu node không có khóa học liên quan, tự động mở khóa
            if (!relatedCourseIds.Any())
            {
                // Chỉ kiểm tra prerequisites, không cần check course purchase
                if (prerequisites.Any())
                {
                    var allPrerequisitesCompleted = prerequisites.All(prereq =>
                        nodeStatusMap.ContainsKey(prereq) && (nodeStatusMap[prereq] == "available" || nodeStatusMap[prereq] == "completed" || nodeStatusMap[prereq] == "in-progress")
                    );

                    if (!allPrerequisitesCompleted)
                    {
                        return "locked";
                    }
                }

                return "available";
            }

            // Logic cũ cho nodes có khóa học
            if (relatedCourseIds.Any() && relatedCourseIds.All(cid => completedCourseIds.Contains(cid)))
            {
                return "completed";
            }


            var hasPurchasedCourse = relatedCourseIds.Any() && relatedCourseIds.Any(cid => purchasedCourseIds.Contains(cid));

            if (hasPurchasedCourse)
            {
                return "available";
            }

            if (prerequisites.Any())
            {
                var allPrerequisitesCompleted = prerequisites.All(prereq =>
                    nodeStatusMap.ContainsKey(prereq) && nodeStatusMap[prereq] == "available"
                );

                if (!allPrerequisitesCompleted)
                {
                    return "locked";
                }
            }

            return "available";
        }

        private List<string> ParseJsonArray(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return new List<string>();
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private List<ResourceDto> ParseResources(string json, HashSet<long> purchasedCourseIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return new List<ResourceDto>();

                var resources = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json) ?? new List<Dictionary<string, object>>();

                return resources.Select(r => new ResourceDto
                {
                    Id = r.ContainsKey("id") ? r["id"].ToString() : Guid.NewGuid().ToString(),
                    Name = r.ContainsKey("name") ? r["name"].ToString() : "",
                    Url = r.ContainsKey("url") ? r["url"].ToString() : "",
                    CourseId = r.ContainsKey("courseId") && long.TryParse(r["courseId"].ToString(), out var cid) ? cid : null,
                    IsPurchased = r.ContainsKey("courseId") && long.TryParse(r["courseId"].ToString(), out var courseId) && purchasedCourseIds.Contains(courseId)
                }).ToList();
            }
            catch
            {
                return new List<ResourceDto>();
            }
        }

        private RoadmapStatisticsDto CalculateStatistics(List<RoadmapNodeDto> nodes, int totalPurchased, int totalCompleted)
        {
            var totalNodes = nodes.Count;
            var completedNodes = nodes.Count(n => n.Data.Status == "completed");
            var inProgressNodes = nodes.Count(n => n.Data.Status == "in-progress");
            var availableNodes = nodes.Count(n => n.Data.Status == "available");
            var lockedNodes = nodes.Count(n => n.Data.Status == "locked");

            var completionPercent = totalNodes > 0 ? (int)((completedNodes * 100.0) / totalNodes) : 0;

            return new RoadmapStatisticsDto
            {
                TotalNodes = totalNodes,
                CompletedNodes = completedNodes,
                InProgressNodes = inProgressNodes,
                AvailableNodes = availableNodes,
                LockedNodes = lockedNodes,
                CompletionPercent = completionPercent,
                TotalCoursesPurchased = totalPurchased,
                TotalCoursesCompleted = totalCompleted
            };
        }

        public async Task<bool> UpdateNodeProgress(long userId, long nodeId, int progressPercent)
        {
            var progress = await _dbContext.Set<UserKnowledgeProgressModel>()
                .FirstOrDefaultAsync(up => up.UserId == userId && up.NodeId == nodeId);

            if (progress == null)
            {
                // Get node to get subjectId
                var node = await _dbContext.KGNodes.FindAsync(nodeId);
                if (node == null) return false;

                progress = new UserKnowledgeProgressModel
                {
                    UserId = userId,
                    NodeId = nodeId,
                    SubjectId = node.SubjectId,
                    Status = "in-progress",
                    ProgressPercent = progressPercent,
                    StartedAt = Utils.GetCurrentVNTime(),
                    LastAccessedAt = Utils.GetCurrentVNTime(),
                    CreatedAt = Utils.GetCurrentVNTime()
                };
                await _dbContext.Set<UserKnowledgeProgressModel>().AddAsync(progress);
            }
            else
            {
                progress.ProgressPercent = progressPercent;
                progress.LastAccessedAt = Utils.GetCurrentVNTime();
                progress.UpdatedAt = Utils.GetCurrentVNTime();

                if (progressPercent >= 100 && progress.Status != "completed")
                {
                    progress.Status = "completed";
                    progress.CompletedAt = Utils.GetCurrentVNTime();
                }
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkNodeAsCompleted(long userId, long nodeId)
        {
            return await UpdateNodeProgress(userId, nodeId, 100);
        }

        public async Task<bool> StartNode(long userId, long nodeId)
        {
            var progress = await _dbContext.Set<UserKnowledgeProgressModel>()
                .FirstOrDefaultAsync(up => up.UserId == userId && up.NodeId == nodeId);

            if (progress != null) return true; // Already started

            var node = await _dbContext.KGNodes.FindAsync(nodeId);
            if (node == null) return false;

            progress = new UserKnowledgeProgressModel
            {
                UserId = userId,
                NodeId = nodeId,
                SubjectId = node.SubjectId,
                Status = "in-progress",
                ProgressPercent = 0,
                StartedAt = Utils.GetCurrentVNTime(),
                LastAccessedAt = Utils.GetCurrentVNTime(),
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<UserKnowledgeProgressModel>().AddAsync(progress);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ImportKnowledgeGraph(ALAP.Entity.DTO.Request.ImportKnowledgeGraphDto dto)
        {
            return await TransactionAsync(async () =>
            {
                // Check if subject already exists
                var existingSubject = await _dbContext.KGSubjects
                    .Include(s => s.Nodes)
                    .Include(s => s.Edges)
                    .FirstOrDefaultAsync(s => s.SubjectId == dto.SubjectId);

                if (existingSubject != null)
                {
                    // Delete existing edges first (to avoid FK constraint issues), then nodes
                    _dbContext.KGEdges.RemoveRange(existingSubject.Edges);
                    _dbContext.KGNodes.RemoveRange(existingSubject.Nodes);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    // Create new subject
                    existingSubject = new KGSubjectModel
                    {
                        SubjectId = dto.SubjectId,
                        SubjectCode = dto.SubjectCode,
                        SubjectName = dto.SubjectName,
                        Description = dto.Description,
                        Version = "1.0",
                        IsActive = true,
                        CreatedAt = Utils.GetCurrentVNTime()
                    };
                    await _dbContext.KGSubjects.AddAsync(existingSubject);
                    await _dbContext.SaveChangesAsync();
                }

                // Import nodes
                var nodeIdMap = new Dictionary<string, long>(); // Map from nodeCode to nodeId
                long nodeIdCounter = 1;

                foreach (var nodeDto in dto.Nodes)
                {
                    var node = new KGNodeModel
                    {
                        NodeId = nodeIdCounter++,
                        SubjectId = existingSubject.Id,
                        NodeCode = nodeDto.Id ?? $"node{nodeIdCounter}",
                        PositionX = nodeDto.X,
                        PositionY = nodeDto.Y,
                        Label = nodeDto.Label ?? "Untitled",
                        Category = nodeDto.Category ?? "foundation",
                        Description = nodeDto.Description ?? "",
                        EstimatedTime = nodeDto.EstimatedTime ?? "1-2 tuần",
                        Difficulty = nodeDto.Difficulty ?? "Cơ bản",
                        Status = "available",
                        ConceptsJson = JsonSerializer.Serialize(nodeDto.Concepts ?? new List<string>()),
                        ExamplesJson = JsonSerializer.Serialize(nodeDto.Examples ?? new List<string>()),
                        PrerequisitesJson = JsonSerializer.Serialize(nodeDto.Prerequisites ?? new List<string>()),
                        ResourcesJson = JsonSerializer.Serialize(nodeDto.Resources ?? new List<ImportResourceDto>()),
                        CreatedAt = Utils.GetCurrentVNTime()
                    };

                    await _dbContext.KGNodes.AddAsync(node);
                    await _dbContext.SaveChangesAsync();

                    nodeIdMap[nodeDto.Id ?? $"node{nodeIdCounter}"] = node.Id;
                }

                // Import edges
                long edgeIdCounter = 1;
                foreach (var edgeDto in dto.Edges)
                {
                    var source = edgeDto.Source ?? "";
                    var target = edgeDto.Target ?? "";

                    if (nodeIdMap.ContainsKey(source) && nodeIdMap.ContainsKey(target))
                    {
                        var edge = new KGEdgeModel
                        {
                            EdgeId = edgeIdCounter++,
                            SubjectId = existingSubject.Id,
                            EdgeCode = edgeDto.Id ?? $"edge{edgeIdCounter}",
                            SourceNodeId = nodeIdMap[source],
                            TargetNodeId = nodeIdMap[target],
                            EdgeType = edgeDto.Type ?? "required",
                            CreatedAt = Utils.GetCurrentVNTime()
                        };

                        await _dbContext.KGEdges.AddAsync(edge);
                    }
                }

                await _dbContext.SaveChangesAsync();
                return true;
            });
        }
    }
}
