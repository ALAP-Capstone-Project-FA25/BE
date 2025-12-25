using ALAP.BizLogic.Implement;
using ALAP.BLL.BackgroundServices;
using ALAP.BLL.Implement;
using ALAP.BLL.Interface;
using ALAP.DAL.Database;
using ALAP.DAL.Implement;
using ALAP.DAL.Interface;
using ALAP.API.MiddleWare;
using System.ComponentModel;

namespace ALAP.API.DependencyConfig
{
    public class DependencyConfig
    {
        public static void Register(IServiceCollection services)
        {
            //General
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            ////BLL
            services.AddScoped<IIdentityBiz, IdentityBiz>();
            services.AddScoped<ICategoryBizLogic, CategoryBizLogic>();
            services.AddScoped<ICourseBizLogic, CourseBizLogic>();
            services.AddScoped<ITopicBizLogic, TopicBizLogic>();
            services.AddScoped<ILessonBizLogic, LessonBizLogic>();
            services.AddScoped<IPackageBizLogic, PackageBizLogic>();
            services.AddScoped<IUserCourseProgressBizLogic, UserCourseProgressBizLogic>();
            services.AddScoped<IUserTopicProgressBizLogic, UserTopicProgressBizLogic>();
            services.AddScoped<IUserLessonProgressBizLogic, UserLessonProgressBizLogic>();
            services.AddScoped<IPaymentBizLogic, PaymentBizLogic>();
            services.AddScoped<IUserPackageBizLogic, UserPackageBizLogic>();
            services.AddScoped<ILoginHistoryBizLogic, LoginHistoryBizLogic>();
            services.AddScoped<ITopicQuestionBizLogic, TopicQuestionBizLogic>();
            services.AddScoped<ITopicQuestionAnswerBizLogic, TopicQuestionAnswerBizLogic>();
            services.AddScoped<IChatRoomBizLogic, ChatRoomBizLogic>();
            services.AddScoped<IChatRoomMessageBizLogic, ChatRoomMessageBizLogic>();
            services.AddScoped<ILessonNoteBizLogic, LessonNoteBizLogic>();
            services.AddScoped<IEventBizLogic, EventBizLogic>();
            services.AddScoped<IEventTicketBizLogic, EventTicketBizLogic>();
            services.AddScoped<IEmailBizLogic, EmailBizLogic>();
            services.AddScoped<IKGBizLogic, KGBizLogic>();
            services.AddScoped<IMajorBizLogic, MajorBizLogic>();
            services.AddScoped<IEntryTestBizLogic, EntryTestBizLogic>();
            services.AddScoped<IKnowledgeGraphBizLogic, KnowledgeGraphBizLogic>();
            services.AddScoped<IDashboardBizLogic, DashboardBizLogic>();
            services.AddScoped<IMentorAuthorizationService, MentorAuthorizationService>();
            services.AddScoped<ITopicQuizBizLogic, TopicQuizBizLogic>();
            services.AddScoped<IAdaptiveRecommendationBizLogic, AdaptiveRecommendationBizLogic>();
            services.AddScoped<INotificationBizLogic, NotificationBizLogic>();
            services.AddScoped<ISearchBizLogic, SearchBizLogic>();
            services.AddScoped<IBlogPostBizLogic, BlogPostBizLogic>();

            // Background Services & Email Queue
            services.AddSingleton<IBackgroundEmailQueue, BackgroundEmailQueue>();
            services.AddScoped<IEventTransitionService, EventTransitionService>();

            // DAL
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IUserCourseRepository, UserCourseRepository>();
            services.AddScoped<IUserTopicRepository, UserTopicRepository>();
            services.AddScoped<IUserLessonRepository, UserLessonRepository>();
            services.AddScoped<ITopicQuestionRepository, TopicQuestionRepository>();
            services.AddScoped<ITopicQuestionAnswerRepository, TopicQuestionAnswerRepository>();
            services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
            services.AddScoped<IChatRoomMessageRepository, ChatRoomMessageRepository>();
            services.AddScoped<ILessonNoteRepository, LessonNoteRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventTicketRepository, EventTicketRepository>();
            services.AddScoped<IKGRepository, KGRepository>();
            services.AddScoped<IMajorRepository, MajorRepository>();
            services.AddScoped<IEntryTestRepository, EntryTestRepository>();
            services.AddScoped<ITopicQuizRepository, TopicQuizRepository>();
            services.AddScoped<IUserWeakAreaRepository, UserWeakAreaRepository>();
            services.AddScoped<IBlogPostRepository, BlogPostRepository>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();
            // Mail
            services.AddScoped<IEmailService, EmailService>();
            services.AddHostedService<EmailSenderWorkerService>();
            //JWT
            services.AddScoped<JWTAuthenticationMiddleware>();


            //worker
            //services.AddHostedService<OrderWoker>();


            // DB
            services.AddDbContext<BaseDBContext>();

        }
    }
}
