namespace ALAP.Entity.Models.Enums
{
    public enum NotificationType
    {
        PAYMENT_SUCCESS = 1,              // Thanh toán thành công
        PACKAGE_PURCHASE_SUCCESS = 2,     // Mua gói thành công
        EVENT_TICKET_PURCHASE_SUCCESS = 3, // Mua vé event thành công
        KNOWLEDGE_REINFORCEMENT = 4,      // Có kiến thức củng cố mới
        EVENT_UPCOMING = 5,              // Event sắp đến hạn
        EVENT_STARTED = 6,                // Event đang diễn ra
        EVENT_ENDED = 7,                  // Event hết hạn
        MENTOR_MESSAGE = 8,               // Mentor nhắn tin
        REFUND_PROCESSED = 9,             // Hoàn tiền từ hệ thống
        ACCOUNT_REGISTERED = 10,          // Đăng ký tài khoản thành công
        ACCOUNT_VERIFIED = 11,            // Xác thực tài khoản thành công
        BLOG_COMMENT_REPLY = 12,         // Có người reply vào comment của bạn
        BLOG_POST_LIKED = 13             // Bài viết của bạn được like
    }
}
