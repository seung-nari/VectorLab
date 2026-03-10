using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VectorLab.Models;

namespace VectorLab.Services
{
    internal static class LabelFileService
    {
        private static readonly string ConfigFolder = Path.Combine(AppContext.BaseDirectory, "conf");

        private static readonly string LabelFilePath = Path.Combine(ConfigFolder, "Labels.json");
        
        // json 전체 구조
        // 이미지 경로 -> 라벨 목록
        private static Dictionary<string, List<LabelRect>> LoadAll()
        {
            if(!File.Exists(LabelFilePath)) return new Dictionary<string, List<LabelRect>>();

            string json = File.ReadAllText(LabelFilePath);

            return JsonSerializer.Deserialize<Dictionary<string, List<LabelRect>>>(json) ?? new Dictionary<string, List<LabelRect>>();
        } 

        // json 전체 저장
        private static void SaveAll(Dictionary<string, List<LabelRect>> data)
        {
            if(!Directory.Exists(ConfigFolder)) Directory.CreateDirectory(ConfigFolder);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            string json = JsonSerializer.Serialize(data, options);

            File.WriteAllText(LabelFilePath, json);
        }

        // 특정 이미지 라벨 저장
        public static void Save(string imagePath, IEnumerable<LabelRect> labels)
        {
            var all = LoadAll();

            all[imagePath] = new List<LabelRect>(labels);

            SaveAll(all);
        }

        // 특정 이미지 라벨 로드
        public static List<LabelRect> Load(string imagePath)
        {
            var all = LoadAll();

            if (all.TryGetValue(imagePath, out var labels)) return labels;

            return new List<LabelRect>();
        }
    }
}
