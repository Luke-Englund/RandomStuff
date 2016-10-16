namespace SortingAlgos
{
    public static class MergeSort
    {
        public static void Sort(int[] arr)
        {
            Sort(arr, 0, arr.Length - 1);
        }

        private static void Sort(int[] arr, int start, int end)
        {
            if (start == end) return;

            var middle = (start + end - 1)/2;

            Sort(arr, start, middle);
            Sort(arr, middle + 1, end);

            Merge(arr, start, middle + 1, end);
        }

        private static void Merge(int[] arr, int start, int middle, int end)
        {
            var idx1 = start;
            var idx2 = middle;
            var tmpIdx = 0;
            var tempArr = new int[end - start + 1];

            while (idx1 < middle && idx2 <= end)
            {
                tempArr[tmpIdx++] = (arr[idx1] < arr[idx2]) ? arr[idx1++] : arr[idx2++];
            }

            while (idx1 < middle)
            {
                tempArr[tmpIdx++] = arr[idx1++];
            }

            while (idx2 <= end)
            {
                tempArr[tmpIdx++] = arr[idx2++];
            }

            for (tmpIdx = 0; tmpIdx < tempArr.Length; tmpIdx++)
            {
                arr[start + tmpIdx] = tempArr[tmpIdx];
            }
        }
    }
}