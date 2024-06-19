using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace csr_windows.Common.Utils
{
    public class StringProcessor
    {
        public static string RemoveEmojis(string input)
        {
            // Regex pattern for matching emojis
            var emojiPattern = new Regex("[\uD83C-\uDBFF\uDC00-\uDFFF]+", RegexOptions.Compiled);

            return emojiPattern.Replace(input, string.Empty);
        }

        public static List<string> RemoveStringsWithEmoji(List<string> stringList)
        {
            return stringList.Select(RemoveEmojis).ToList();
        }

        public static List<string> RemoveSpecificStrings(List<string> stringsList)
        {
            List<string> keywords = new List<string>
       {
           "此外", "同时", "不过", "另外", "设计上", "而且", "例如", "首先", "目前", "比如", "当然", "但是"
       };

            return stringsList.Where(s => !keywords.Contains(s)).ToList();
        }

        public static List<List<string>> Divide(string res)
        {
            var sentences = new List<List<string>>();
            var splitSentences = res.Split('。');

            foreach (var s in splitSentences)
            {
                List<string> sentence;
                if (s.Length > 60)
                {
                    sentence = Regex.Split(s, @"[。；？！～]|[，。；？！～](?=”)|(?=”)[，。；？！～]|(?<=哦|啊|啦|呢)，(?=.{4,}。)|，(?=灵感)|，(?=色彩)|，(?=整体设计)|，(?=每一件)|，(?=这个)|，(?=设计理念)|，(?=设计风格)|，(?=设计灵感)|，(?=同时)|，(?=首先)|，(?=这种)|，(?=只要)|，(?=关于)|，(?=.{0,2}建议)|，(?=这.{0,1}款)|，(?=如果)|，(?=您)|，(?=.{0,2}因为)|，(?=虽然)|，(?=不过)|，(?=这样)|，(?=比如)|，(?=它)|，(?=我们)|，(?=采用)|，(?=无论)|，(?=即使)|，(?=不仅)").ToList();
                }
                else
                {
                    sentence = Regex.Split(s, @"[。；？！～]|[，。；？！～](?=”)|(?=”)[，。；？！～]").ToList();
                }

                var removeEmojiSentence = RemoveStringsWithEmoji(sentence);
                var divideSentence = RemoveSpecificStrings(removeEmojiSentence);
                sentences.Add(divideSentence);
            }

            return sentences;
        }
    }
}
