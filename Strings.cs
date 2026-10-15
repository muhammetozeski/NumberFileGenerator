using System.Globalization;

namespace NumberFileGenerator;

/// <summary>Embedded console messages; selecting the system language never writes a language file.</summary>
static class Strings
{
    static bool Turkish => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "tr";
    public static string Description => Turkish ? "Ardışık sayıları EXE'nin yanına yazar. 1 GB = 1.073.741.824 bayt." : "Writes consecutive numbers beside the EXE. 1 GB = 1,073,741,824 bytes.";
    public static string Prompt => Turkish ? "Dosya boyutu (GB, örnek: 1,5): " : "File size (GB, example: 1,5): ";
    public static string InvalidSize => Turkish ? "Virgüllü veya tam, pozitif bir sayı girin." : "Enter a positive decimal or whole number.";
    public static string NoSpace => Turkish ? "Seçilen boyut için diskte yeterli boş alan yok." : "There is not enough free disk space for the requested size.";
    public static string Destination => Turkish ? "Dosya: {0}" : "File: {0}";
    public static string Progress => Turkish ? "%{0:F1} — {1:F2} GB yazıldı" : "{0:F1}% — {1:F2} GB written";
    public static string Completed => Turkish ? "Tamamlandı: {0:N0} bayt, son sayı {1:N0}, süre {2:F2} saniye." : "Completed: {0:N0} bytes, last number {1:N0}, elapsed {2:F2} seconds.";
    public static string Partial => Turkish ? "Kısmi dosya korundu: {0} ({1:N0} bayt)." : "Partial file preserved: {0} ({1:N0} bytes).";
    public static string Cancelled => Turkish ? "İşlem iptal edildi." : "Operation cancelled.";
    public static string Failed => Turkish ? "Dosya oluşturulamadı. Hata ayrıntısı:" : "The file could not be created. Error details:";
    public static string WrongLength => Turkish ? "Yazılan dosyanın boyutu istenen boyutla eşleşmiyor." : "The written file size does not match the requested size.";
    public static string Close => Turkish ? "Kapatmak için Enter'a basın." : "Press Enter to close.";
}
