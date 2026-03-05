using System.ComponentModel.DataAnnotations;

namespace FuncProject
{
    public class HelloRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
