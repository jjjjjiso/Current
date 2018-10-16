using System;
using System.Linq;

namespace WaterBlast.System
{
    /// <summary>
    /// Miscellaneous reflection utilities.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// 지정된 형식에서 파생 된 모든 형식을 반환합니다.
        /// </summary>
        /// <param name="aAppDomain">앱 도메인.</param>
        /// <param name="aType">기본 유형.</param>
        /// <returns>지정된 유형에서 파생 된 모든 유형.</returns>
        public static Type[] GetAllDerivedTypes(this AppDomain aAppDomain, Type aType)
        {
            var assemblies = aAppDomain.GetAssemblies();
            return (from assembly in assemblies
                    from type in assembly.GetTypes()
                    where type.IsSubclassOf(aType)
                    select type).ToArray();
        }
    }
}
