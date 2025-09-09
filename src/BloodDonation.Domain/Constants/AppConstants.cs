namespace BloodDonation.Domain.Constants;

public static class AppConstants  
{
    // so ngay toi thieu giua 2 lan hien mau
    public const int MIN_DAYS_BETWEEN_DONATIONS = 84; // 12 weeks
    
    // TODO: move to config file
    public const int MIN_AGE = 18;
    public const int MAX_AGE = 60;  // co the len 65?
    
    public const decimal MIN_WEIGHT = 45.0m;  // kg
    public const decimal MIN_WEIGHT_MALE = 50.0m; // nam phai >= 50kg
    
    // blood amount
    public const decimal DEFAULT_DONATION_AMOUNT = 350;  // ml
    public const decimal MAX_DONATION_AMOUNT = 450;
    
    // test values - xoa sau khi deploy
    //public const string TEST_USER = "admin";
    //public const string TEST_PWD = "Admin@123";
    
    // time slots cho appointment
    public static readonly string[] TimeSlots = new[]
    {
        "08:00-08:30",
        "08:30-09:00",
        "09:00-09:30",
        "09:30-10:00",
        "10:00-10:30",
        "10:30-11:00",
        // break trua
        "13:00-13:30",
        "13:30-14:00",
        "14:00-14:30",
        "14:30-15:00",
        "15:00-15:30",
        "15:30-16:00"
    };
    
    // msg templates - can sua lai cho hay hon
    public static class Messages 
    {
        public const string DONATION_SUCCESS = "Cảm ơn bạn đã hiến máu!";
        public const string APPOINTMENT_BOOKED = "Đặt lịch thành công";
        public const string APPOINTMENT_CANCELLED = "Đã hủy lịch hẹn";
        
        // err msgs
        public const string ERR_GENERAL = "Có lỗi xảy ra"; // generic err
        public const string ERR_NOT_FOUND = "Không tìm thấy dữ liệu";
        public const string ERR_INVALID_DATA = "Dữ liệu không hợp lệ";
    }
}
