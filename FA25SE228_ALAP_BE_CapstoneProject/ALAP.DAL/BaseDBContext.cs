using ALAP.DAL.Implement;
using ALAP.DAL.Interface;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ALAP.DAL
{
    public class BaseDBContext : DbContext
    {
        public BaseDBContext(DbContextOptions<BaseDBContext> options) : base(options)
        {

        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<TopicModel> Topics { get; set; }
        public DbSet<LessonModel> Lessons { get; set; }
        public DbSet<UserCourseModel> UserCourses { get; set; }
        public DbSet<UserTopicModel> UserTopics { get; set; }
        public DbSet<UserLessonModel> UserLessons { get; set; }
        public DbSet<UserTopicProgressModel> UserTopicProgresses { get; set; }
        public DbSet<PackageModel> Packages { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<UserPackageModel> UserPackages { get; set; }
        public DbSet<LoginHistoryModel> LoginHistories { get; set; }
        public DbSet<TopicQuestionModel> TopicQuestions { get; set; }
        public DbSet<TopicQuestionAnswerModel> TopicQuestionAnswers { get; set; }
        public DbSet<ChatRoomModel> ChatRooms { get; set; }
        public DbSet<ChatRoomMessageModel> ChatRoomMessages { get; set; }
        public DbSet<LessonNoteModel> LessonNotes { get; set; }
        public DbSet<EventModel> Events { get; set; }
        public DbSet<EventTicketModel> EventTickets { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }

        public DbSet<MajorModel> Majors { get; set; }

        public DbSet<KGSubjectModel> KGSubjects { get; set; }
        public DbSet<KGNodeModel> KGNodes { get; set; }
        public DbSet<KGEdgeModel> KGEdges { get; set; }

        public DbSet<EntryTestModel> EntryTests { get; set; }
        public DbSet<EntryTestQuestionModel> EntryTestQuestions { get; set; }
        public DbSet<EntryTestOptionModel> EntryTestOptions { get; set; }
        public DbSet<EntryTestSubjectMappingModel> EntryTestSubjectMappings { get; set; }
        public DbSet<EntryTestResultModel> EntryTestResults { get; set; }
        public DbSet<UserKnowledgeProgressModel> UserKnowledgeProgress { get; set; }
        public DbSet<UserTopicQuizAttempt> UserTopicQuizAttempts { get; set; }
        public DbSet<UserWrongAnswer> UserWrongAnswers { get; set; }
        public DbSet<UserWeakArea> UserWeakAreas { get; set; }

        public DbSet<BlogPostModel> BlogPosts { get; set; }
        public DbSet<BlogPostSectionModel> BlogPostSections { get; set; }
        public DbSet<BlogPostCommentModel> BlogPostComments { get; set; }
        public DbSet<BlogPostLikeModel> BlogPostLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();

            List<UserModel> users = [
                new UserModel()
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "Admin",
                    Email = "admin@gmail.com",
                    Password = passwordHasher.HashPassword("123456"),
                    Username= "admin",
                    EmailConfirmed = true,
                    Role = UserRole.ADMIN,
                    Phone = "0912212312"
                },
                new UserModel()
                {
                    Id = 3,
                    FirstName = "User",
                    LastName = "User",
                    Email = "user@gmail.com",
                    Password = passwordHasher.HashPassword("123456"),
                    Username = "user",
                    EmailConfirmed = true,
                    Phone = "0372599559",
                    Role = UserRole.USER
                },
                new UserModel()
                {
                    Id = 2,
                    FirstName = "Mentor",
                    LastName = "mentor",
                    Email = "mentor@gmail.com",
                    Password = passwordHasher.HashPassword("123456"),
                    Username = "mentor",
                    EmailConfirmed = true,
                    Phone = "0372599559",
                    Role = UserRole.MENTOR
                },
                new UserModel()
                {
                    Id = 6,
                    FirstName = "Speaker",
                    LastName = "Speaker",
                    Email = "speaker@gmail.com",
                    Password = passwordHasher.HashPassword("123456"),
                    Username = "speaker",
                    EmailConfirmed = true,
                    Phone = "0372599559",
                    Role = UserRole.SPEAKER
                }
            ];



            List<PackageModel> packages = new List<PackageModel>
{
    new PackageModel
    {
        Id = 1,
        Title = "Gói cơ bản",
        Description = "Gói cơ bản phù hợp với người mới bắt đầu",
        PackageType = PackageType.STARTER,
        Duration = 30,
        IsActive = true,
        Price = 99000,
        IsPopular = false,
        Features = "[\"Tham gia các khóa học trả phí\", \"Được đóng góp nhận xét từ mentor\"]"
    },
    new PackageModel
    {
        Id = 2,
        Title = "Gói nâng cao",
        Description = "Gói nâng cao, thêm các chức năng nâng cao mới",
        PackageType = PackageType.PREMIUM,
        Duration = 30,
        IsActive = true,
        Price = 199000,
        IsPopular = true,
        Features = "[\"Tham gia các khóa học trả phí\", \"Được đóng góp nhận xét từ mentor\", \"Nói chuyện, trao đổi sâu hơn với mentor\"]"
    }
};

            builder.Entity<PackageModel>().HasData(packages);

            builder.Entity<UserModel>()
            .HasData(users);

            builder.Entity<UserModel>().Property(u => u.Id).HasColumnName("Id");

            // Composite unique indexes to prevent duplicates per user
            builder.Entity<UserCourseModel>()
                .HasIndex(x => new { x.UserId, x.CourseId })
                .IsUnique();

            builder.Entity<UserTopicModel>()
                .HasIndex(x => new { x.UserCourseId, x.TopicId })
                .IsUnique();

            builder.Entity<UserLessonModel>()
                .HasIndex(x => new { x.UserTopicId, x.LessonId })
                .IsUnique();

            // Unique index for UserWeakAreas (UserId, LessonId)
            builder.Entity<UserWeakArea>()
                .HasIndex(x => new { x.UserId, x.LessonId })
                .IsUnique();

            // Indexes cho Event Status Transitions (Background Job Performance)
            builder.Entity<EventModel>()
                .HasIndex(e => new { e.Status, e.StartDate })
                .HasDatabaseName("IX_Events_Status_StartDate");

            builder.Entity<EventModel>()
                .HasIndex(e => new { e.Status, e.EndDate })
                .HasDatabaseName("IX_Events_Status_EndDate");

            builder.Entity<EventTicketModel>()
                .HasIndex(et => new { et.EventId, et.IsActive })
                .HasDatabaseName("IX_EventTickets_EventId_IsActive");

            builder.Entity<KGSubjectModel>(entity =>
            {
                entity.ToTable("KGSubjects");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.SubjectCode).HasMaxLength(50).IsRequired();
                entity.Property(x => x.SubjectName).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Description).HasMaxLength(1000);
                entity.Property(x => x.Version).HasMaxLength(20);
                entity.HasIndex(x => x.SubjectCode).IsUnique();
            });

            // KG Node
            builder.Entity<KGNodeModel>(entity =>
            {
                entity.ToTable("KGNodes");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.NodeCode).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Label).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Category).HasMaxLength(50);
                entity.Property(x => x.Description).HasMaxLength(2000);
                entity.Property(x => x.EstimatedTime).HasMaxLength(50);
                entity.Property(x => x.Difficulty).HasMaxLength(50);
                entity.Property(x => x.Status).HasMaxLength(20);

                entity.HasOne(x => x.Subject)
                    .WithMany(s => s.Nodes)
                    .HasForeignKey(x => x.SubjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new { x.SubjectId, x.NodeCode }).IsUnique();
            });

            // KG Edge
            builder.Entity<KGEdgeModel>(entity =>
            {
                entity.ToTable("KGEdges");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.EdgeCode).HasMaxLength(100).IsRequired();
                entity.Property(x => x.EdgeType).HasMaxLength(50);

                entity.HasOne(x => x.Subject)
                    .WithMany(s => s.Edges)
                    .HasForeignKey(x => x.SubjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.SourceNode)
                    .WithMany(n => n.OutgoingEdges)
                    .HasForeignKey(x => x.SourceNodeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.TargetNode)
                    .WithMany(n => n.IncomingEdges)
                    .HasForeignKey(x => x.TargetNodeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.SubjectId, x.EdgeCode }).IsUnique();
            });

            // BlogPost configuration
            builder.Entity<BlogPostModel>(entity =>
            {
                entity.ToTable("BlogPosts");
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Author)
                    .WithMany()
                    .HasForeignKey(x => x.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.TargetAudience).HasDatabaseName("IX_BlogPosts_TargetAudience");
                entity.HasIndex(x => x.AuthorId).HasDatabaseName("IX_BlogPosts_AuthorId");
                entity.HasIndex(x => x.CreatedAt).HasDatabaseName("IX_BlogPosts_CreatedAt");
                entity.HasIndex(x => x.IsActive).HasDatabaseName("IX_BlogPosts_IsActive");
            });

            // BlogPostSection configuration
            builder.Entity<BlogPostSectionModel>(entity =>
            {
                entity.ToTable("BlogPostSections");
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.BlogPost)
                    .WithMany(p => p.Sections)
                    .HasForeignKey(x => x.BlogPostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new { x.BlogPostId, x.OrderIndex }).HasDatabaseName("IX_BlogPostSections_BlogPostId_OrderIndex");
            });

            // BlogPostComment configuration
            builder.Entity<BlogPostCommentModel>(entity =>
            {
                entity.ToTable("BlogPostComments");
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.BlogPost)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(x => x.BlogPostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ParentComment)
                    .WithMany(p => p.Replies)
                    .HasForeignKey(x => x.ParentCommentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => x.BlogPostId).HasDatabaseName("IX_BlogPostComments_BlogPostId");
                entity.HasIndex(x => x.CreatedAt).HasDatabaseName("IX_BlogPostComments_CreatedAt");
                entity.HasIndex(x => x.ParentCommentId).HasDatabaseName("IX_BlogPostComments_ParentCommentId");
            });

            // BlogPostLike configuration
            builder.Entity<BlogPostLikeModel>(entity =>
            {
                entity.ToTable("BlogPostLikes");
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.BlogPost)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(x => x.BlogPostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint: one user can like a post only once
                entity.HasIndex(x => new { x.UserId, x.BlogPostId })
                    .IsUnique()
                    .HasDatabaseName("IX_BlogPostLikes_UserId_BlogPostId");
            });
        }





    }
}
