using System.Text.RegularExpressions;

namespace BloodDonation.Application.Helpers;

public static class ValidationHelper
{
    // check sdt VN
    public static bool IsValidPhoneNumber(string phone)
    {
        if (string.IsNullOrEmpty(phone)) return false;
        
        // pattern cho sdt vn - can test lai
        var pattern = @"^(0|\+84)[3|5|7|8|9][0-9]{8}$";
        return Regex.IsMatch(phone, pattern);
    }
    
    // validate cccd - chua lam xong
    public static bool IsValidIdentityNumber(string id)
    {
        // TODO: implement proper validation
        // tam thoi check do dai thoi
        if(string.IsNullOrEmpty(id))
            return false;
            
        return id.Length == 9 || id.Length == 12;  // cmnd 9 so, cccd 12 so
    }
    
    public static bool IsValidEmail(string email)
    {
        try 
        {
            // dung built-in cua .NET cho nhanh
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    // ham nay bi loi - fix sau
    /*
    public static int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }
    */
    
    // HACK: temporary solution
    public static bool CheckAge(DateTime? dob)
    {
        if(!dob.HasValue) return true; // ko co ngay sinh thi cho qua luon :))
        
        var age = DateTime.Now.Year - dob.Value.Year;
        return age >= 18 && age <= 60;
    }
}
