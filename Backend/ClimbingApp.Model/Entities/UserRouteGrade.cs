using System.Text.Json.Serialization;

namespace ClimbingApp.Model.Entities
{
    public class UserRouteGrade
    {
        [JsonPropertyName("userid")]
        public int? UserID { get; set; }
        
        [JsonPropertyName("routeid")]
        public int RouteID { get; set; }
        
        [JsonPropertyName("gradefbleau")]
        public string GradeFbleau { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }
        
        [JsonPropertyName("gymid")]
        public int GymID { get; set; }
        
        [JsonPropertyName("setdate")]
        public string SetDate { get; set; }
        
        [JsonPropertyName("removedate")]
        public string RemoveDate { get; set; }
        
        [JsonPropertyName("adminid")]
        public int AdminID { get; set; }
    }
}

