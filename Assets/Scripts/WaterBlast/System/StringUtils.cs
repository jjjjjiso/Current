using System.Collections.Generic;
using System.Linq;

namespace WaterBlast.System
{
    /// <summary>
    ///기타 문자열 관련 유틸리티 모음입니다.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// 지정된 문자열을 읽을 수있는 문자열로 반환합니다.
        /// </summary>
        /// <param name="camelCase">The original, camel case string.</param>
        /// <returns>원래 문자열의 읽을 수있는 버전.</returns>
        public static string DisplayCamelCaseString(string camelCase)
        {
            var chars = new List<char> { camelCase[0] };
            foreach (var c in camelCase.Skip(1))
            {
                if (char.IsUpper(c))
                {
                    chars.Add(' ');
                    chars.Add(char.ToLower(c));
                }
                else
                {
                    chars.Add(c);
                }
            }

            return new string(chars.ToArray());
        }
    }
}

