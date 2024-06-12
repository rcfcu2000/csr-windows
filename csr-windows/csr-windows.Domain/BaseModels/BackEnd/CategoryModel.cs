using Newtonsoft.Json;

namespace csr_windows.Domain.BaseModels.BackEnd
{
    public class Category
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 家居布艺
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Industry { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("parentId")]
        public int ParentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("presetText")]
        public string PresetText { get; set; }
    }
}