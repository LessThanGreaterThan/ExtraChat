namespace ExtraChat;

internal static class ArrayExtensions {
    internal static byte[] Concat(this byte[] a, byte[] b) {
        var result = new byte[a.Length + b.Length];

        var idx = 0;
        foreach (var t in a) {
            result[idx++] = t;
        }

        foreach (var t in b) {
            result[idx++] = t;
        }

        return result;
    }
}
