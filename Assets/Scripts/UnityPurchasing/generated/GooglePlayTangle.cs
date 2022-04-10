// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("E+0W3IX8j11ZTjWD+zj+8rLQHP5T3Tye6esW5bX+ez2EcK6ExYYGmzIoDOQ8CWnMqWFuXiMIXaPKZjQRXd7Q3+9d3tXdXd7e3xpJCgl/UjfvXd7979LZ1vVZl1ko0t7e3trf3K09RLDE9H3LfUJe9VU/YTTT6k5vUuYZD7GFZVy3JYwGzeG2ViN/nCnKoqTrp6rcAMucPLDSC5phnsNs2UBrjGAPGzMNcMTueJJViLOVKkovnySEVD5Kq05dpJ8OsSr4kGUgS82l9hRUC9Z0E9/7Gd7Hz0oA+r4/DXjmKxcoRYEkijMQGiYQ/5IaeIMBkR2jlFDsdzYg0R2QcIhwgTBpsTezAzCUnprNIho6wBmjkuQ0AiH79ywIJXugMgJyrt3c3t/e");
        private static int[] order = new int[] { 9,5,4,5,9,5,10,7,11,12,10,12,12,13,14 };
        private static int key = 223;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
