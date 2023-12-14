//This is only for the specific use case in Admin/TeacherList
//where it needed to return a list of Teachers all the while
//declaring a singular teacher model
namespace WebsiteQuanLyLamViecNhom.HelperClasses.TempModels
{
    public class TeacherDynamicModels
    {
        public IEnumerable<Models.Teacher>? Teachers { get; set; }
        public Models.Teacher? Teacher { get; set; }

    }

}
