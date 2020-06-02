
namespace ZPF
{
   public interface I18n_Interface
   {
      string Key { get; set; }
      string Value { get; set; }
      string Language { get; set; }
      string App { get; set; }
      string Source { get; set; }
      bool? Missing { get; set; }
      bool? Validated { get; set; }
   }
}
