// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("qAb4t2ZMoD9iBTlCQ/Up5WsS4w5LBxY3w+llzZhCYVPDD9N1B6Z6KMzvjUNM/reTJdbI2qWuXUOrx4wtpY0o61pEyrIYKfGkfqFFLzUn9iqsbhQke/B3GSCCGu2D3NA1jXr1nHpQ9NzpV8GvposrO0qv5zd+MCAspReUt6WYk5y/E90TYpiUlJSQlZYxCX+y5RGqYLC6lzwDmOrfUQJJnAOkU7QQB7APS/YcMd+31b9kxIsQlMxxbLCQmZo2mYMf3Mo0Sr268enGmCaw8Wbi1pDRuOBJNtB2Ua070o4Gt+snBer3acQ35vzj9edJKAO/F5SalaUXlJ+XF5SUlVM6UNMENXYOrGSQcuIwi44p6VELVqwxXTNst5l8YYAEVjO1npeWlJWU");
        private static int[] order = new int[] { 7,10,4,5,12,11,7,9,11,13,13,12,13,13,14 };
        private static int key = 149;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
