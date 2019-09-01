using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace liu
{
    /// <summary>
    /// 表示包围盒一条边的结构体，里面就只记录两个顶点
    /// </summary>
    public struct BoundSide
    {
        public Vector3 p1;
        public Vector3 p2;
    }

    /// <summary>
    /// 关于Bounds的扩展方法
    /// </summary>
    public static class BoundsExtensions
    {
        /// <summary>
        /// 获取Bounds(待删除)
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static Bounds GetBounds(this List<Vector3> vertices)
        {
            var min_x = vertices.Min(p => p.x);
            var max_x = vertices.Max(p => p.x);
            var min_y = vertices.Min(p => p.y);
            var max_y = vertices.Max(p => p.y);
            var min_z = vertices.Min(p => p.z);
            var max_z = vertices.Max(p => p.z);

            var min = new Vector3(min_x, min_y, min_z);
            var max = new Vector3(max_x, max_y, max_z);

            var center = (max + min) * 0.5F;
            var size = max - min;

            return new Bounds(center, size);
        }

        /// <summary>
        /// 遍历整个物体的子物体，得到一个总的Bounds。
        /// 要注意这个Bounds的坐标是在世界空间下。
        /// </summary>
        /// <param name="go">任一GameObject物体</param>
        /// <returns>Bounds数据</returns>
        public static Bounds GetBounds(this GameObject go)
        {
            //这里只能先把物体放平，等会再放回来
            Quaternion recRotation = go.transform.rotation;
            go.transform.rotation = Quaternion.identity;
            //得到所有MeshFilter
            MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();

            //如果这个物体不包含Mesh
            if (meshFilters == null || meshFilters.Length == 0)
            {
                Debug.LogWarning("BoundsExtensions.GetBounds():这个物体下不包含 MeshFilter");
                return default(Bounds);
            }

            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < meshFilters.Length; i++)
            {
                //获取每个模型最大的点和最小的点，然后变换到世界空间，为了筛选出模型边框的bounds
                Vector3 minPos_world = meshFilters[i].transform.localToWorldMatrix.MultiplyPoint(meshFilters[i].mesh.bounds.min);
                Vector3 maxPos_world = meshFilters[i].transform.localToWorldMatrix.MultiplyPoint(meshFilters[i].mesh.bounds.max);

                //使用minPos_world和maxPos_world两个点，比对更新min的值
                if (minPos_world.x < min.x) { min.x = minPos_world.x; }
                if (minPos_world.y < min.y) { min.y = minPos_world.y; }
                if (minPos_world.z < min.z) { min.z = minPos_world.z; }
                if (maxPos_world.x < min.x) { min.x = maxPos_world.x; }
                if (maxPos_world.y < min.y) { min.y = maxPos_world.y; }
                if (maxPos_world.z < min.z) { min.z = maxPos_world.z; }

                //使用minPos_world和maxPos_world两个点，比对更新max的值
                if (minPos_world.x > max.x) { max.x = minPos_world.x; }
                if (minPos_world.y > max.y) { max.y = minPos_world.y; }
                if (minPos_world.z > max.z) { max.z = minPos_world.z; }
                if (maxPos_world.x > max.x) { max.x = maxPos_world.x; }
                if (maxPos_world.y > max.y) { max.y = maxPos_world.y; }
                if (maxPos_world.z > max.z) { max.z = maxPos_world.z; }
            }

            SkinnedMeshRenderer[] skins = go.GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skins.Length; i++)
            {
                Vector3 minPos_world = skins[i].transform.localToWorldMatrix.MultiplyPoint(skins[i].sharedMesh.bounds.min);
                Vector3 maxPos_world = skins[i].transform.localToWorldMatrix.MultiplyPoint(skins[i].sharedMesh.bounds.max);
                //使用minPos_world和maxPos_world两个点，比对更新min的值
                if (minPos_world.x < min.x) { min.x = minPos_world.x; }
                if (minPos_world.y < min.y) { min.y = minPos_world.y; }
                if (minPos_world.z < min.z) { min.z = minPos_world.z; }
                if (maxPos_world.x < min.x) { min.x = maxPos_world.x; }
                if (maxPos_world.y < min.y) { min.y = maxPos_world.y; }
                if (maxPos_world.z < min.z) { min.z = maxPos_world.z; }

                //使用minPos_world和maxPos_world两个点，比对更新max的值
                if (minPos_world.x > max.x) { max.x = minPos_world.x; }
                if (minPos_world.y > max.y) { max.y = minPos_world.y; }
                if (minPos_world.z > max.z) { max.z = minPos_world.z; }
                if (maxPos_world.x > max.x) { max.x = maxPos_world.x; }
                if (maxPos_world.y > max.y) { max.y = maxPos_world.y; }
                if (maxPos_world.z > max.z) { max.z = maxPos_world.z; }
            }

            Vector3 center = (max + min) * 0.5F;
            Vector3 size = max - min;

            go.transform.rotation = recRotation;//恢复记录

            return new Bounds(center, size);
        }

        /// <summary>
        /// 遍历一组物体的子物体，得到一个总的Bounds。
        /// 要注意这个Bounds的坐标是在世界空间下。
        /// </summary>
        /// <param name="gos">一组物体</param>
        /// <returns>Bounds数据</returns>
        public static Bounds GetBounds(this GameObject[] gos)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);


            for (int index = 0; index < gos.Length; index++)
            {
                GameObject go = gos[index];
                if (go == null)
                {
                    Debug.LogError("BoundsExtensions.GetBounds():输入参数的物体组下有的物体为null!");
                    return default(Bounds);
                }

                //得到所有MeshFilter
                MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
                //蒙皮上没有网格 直接引用模型的 并没有直接添加在模型上
                SkinnedMeshRenderer[] skins = go.GetComponentsInChildren<SkinnedMeshRenderer>();

                for (int i = 0; i < skins.Length; i++)
                {
                    Vector3 minPos_world = skins[i].transform.localToWorldMatrix.MultiplyPoint(skins[i].sharedMesh.bounds.min);
                    Vector3 maxPos_world = skins[i].transform.localToWorldMatrix.MultiplyPoint(skins[i].sharedMesh.bounds.max);
                    //使用minPos_world和maxPos_world两个点，比对更新min的值
                    if (minPos_world.x < min.x) { min.x = minPos_world.x; }
                    if (minPos_world.y < min.y) { min.y = minPos_world.y; }
                    if (minPos_world.z < min.z) { min.z = minPos_world.z; }
                    if (maxPos_world.x < min.x) { min.x = maxPos_world.x; }
                    if (maxPos_world.y < min.y) { min.y = maxPos_world.y; }
                    if (maxPos_world.z < min.z) { min.z = maxPos_world.z; }

                    //使用minPos_world和maxPos_world两个点，比对更新max的值
                    if (minPos_world.x > max.x) { max.x = minPos_world.x; }
                    if (minPos_world.y > max.y) { max.y = minPos_world.y; }
                    if (minPos_world.z > max.z) { max.z = minPos_world.z; }
                    if (maxPos_world.x > max.x) { max.x = maxPos_world.x; }
                    if (maxPos_world.y > max.y) { max.y = maxPos_world.y; }
                    if (maxPos_world.z > max.z) { max.z = maxPos_world.z; }
                }

                for (int i = 0; i < meshFilters.Length; i++)
                {
                    //获取每个模型最大的点和最小的点，然后变换到世界空间，为了筛选出模型边框的bounds
                    Vector3 minPos_world = meshFilters[i].transform.localToWorldMatrix.MultiplyPoint(meshFilters[i].mesh.bounds.min);
                    Vector3 maxPos_world = meshFilters[i].transform.localToWorldMatrix.MultiplyPoint(meshFilters[i].mesh.bounds.max);

                    //使用minPos_world和maxPos_world两个点，比对更新min的值
                    if (minPos_world.x < min.x) { min.x = minPos_world.x; }
                    if (minPos_world.y < min.y) { min.y = minPos_world.y; }
                    if (minPos_world.z < min.z) { min.z = minPos_world.z; }
                    if (maxPos_world.x < min.x) { min.x = maxPos_world.x; }
                    if (maxPos_world.y < min.y) { min.y = maxPos_world.y; }
                    if (maxPos_world.z < min.z) { min.z = maxPos_world.z; }

                    //使用minPos_world和maxPos_world两个点，比对更新max的值
                    if (minPos_world.x > max.x) { max.x = minPos_world.x; }
                    if (minPos_world.y > max.y) { max.y = minPos_world.y; }
                    if (minPos_world.z > max.z) { max.z = minPos_world.z; }
                    if (maxPos_world.x > max.x) { max.x = maxPos_world.x; }
                    if (maxPos_world.y > max.y) { max.y = maxPos_world.y; }
                    if (maxPos_world.z > max.z) { max.z = maxPos_world.z; }
                }
            }

            //如果这个物体不包含Mesh,即初始化的值一个都没有改动
            if (min.x == float.MaxValue)
            {
                Debug.LogWarning("BoundsExtensions.GetBounds():这个物体组下所有物体都不包含一个MeshFilter");
                return default(Bounds);
            }

            Vector3 center = (max + min) * 0.5F;
            Vector3 size = max - min;

            return new Bounds(center, size);
        }

        /// <summary>
        /// 得到本地边界框，其中以这组物体的第一个物体作为基准节点物体。
        /// 这个Bounds的坐标是在模型本地空间下。
        /// </summary>
        /// <param name="gos">一组物体</param>
        /// <returns></returns>
        public static Bounds GetBoundsLocal(this GameObject[] gos)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            if (gos.Length == 0)
            {
                Debug.LogError("BoundsExtensions.GetBoundsLocal():输入参数错误!");
                return default(Bounds);
            }

            //这里只能先把物体放平，等会再放回来
            Quaternion recRotation = gos[0].transform.rotation;
            gos[0].transform.rotation = Quaternion.identity;
            Vector3 recScale = gos[0].transform.localScale;
            gos[0].transform.localScale = Vector3.one;

            for (int index = 0; index < gos.Length; index++)
            {
                GameObject go = gos[index];
                if (go == null)
                {
                    Debug.LogError("BoundsExtensions.GetBoundsLocal():输入参数的物体组下有的物体为null!");
                    return default(Bounds);
                }

                //得到所有MeshFilter
                MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
                //蒙皮上没有网格 直接引用模型的 并没有直接添加在模型上
                SkinnedMeshRenderer[] skins = go.GetComponentsInChildren<SkinnedMeshRenderer>();

                for (int i = 0; i < skins.Length; i++)
                {
                    Vector3 minPos_world = skins[i].transform.localToWorldMatrix.MultiplyPoint(skins[i].sharedMesh.bounds.min);
                    Vector3 maxPos_world = skins[i].transform.localToWorldMatrix.MultiplyPoint(skins[i].sharedMesh.bounds.max);
                    //使用minPos_world和maxPos_world两个点，比对更新min的值
                    if (minPos_world.x < min.x) { min.x = minPos_world.x; }
                    if (minPos_world.y < min.y) { min.y = minPos_world.y; }
                    if (minPos_world.z < min.z) { min.z = minPos_world.z; }
                    if (maxPos_world.x < min.x) { min.x = maxPos_world.x; }
                    if (maxPos_world.y < min.y) { min.y = maxPos_world.y; }
                    if (maxPos_world.z < min.z) { min.z = maxPos_world.z; }

                    //使用minPos_world和maxPos_world两个点，比对更新max的值
                    if (minPos_world.x > max.x) { max.x = minPos_world.x; }
                    if (minPos_world.y > max.y) { max.y = minPos_world.y; }
                    if (minPos_world.z > max.z) { max.z = minPos_world.z; }
                    if (maxPos_world.x > max.x) { max.x = maxPos_world.x; }
                    if (maxPos_world.y > max.y) { max.y = maxPos_world.y; }
                    if (maxPos_world.z > max.z) { max.z = maxPos_world.z; }
                }

                for (int i = 0; i < meshFilters.Length; i++)
                {
                    //获取每个模型最大的点和最小的点，然后变换到世界空间，为了筛选出模型边框的bounds
                    Vector3 minPos_world = meshFilters[i].transform.localToWorldMatrix.MultiplyPoint(meshFilters[i].mesh.bounds.min);
                    Vector3 maxPos_world = meshFilters[i].transform.localToWorldMatrix.MultiplyPoint(meshFilters[i].mesh.bounds.max);

                    //使用minPos_world和maxPos_world两个点，比对更新min的值
                    if (minPos_world.x < min.x) { min.x = minPos_world.x; }
                    if (minPos_world.y < min.y) { min.y = minPos_world.y; }
                    if (minPos_world.z < min.z) { min.z = minPos_world.z; }
                    if (maxPos_world.x < min.x) { min.x = maxPos_world.x; }
                    if (maxPos_world.y < min.y) { min.y = maxPos_world.y; }
                    if (maxPos_world.z < min.z) { min.z = maxPos_world.z; }

                    //使用minPos_world和maxPos_world两个点，比对更新max的值
                    if (minPos_world.x > max.x) { max.x = minPos_world.x; }
                    if (minPos_world.y > max.y) { max.y = minPos_world.y; }
                    if (minPos_world.z > max.z) { max.z = minPos_world.z; }
                    if (maxPos_world.x > max.x) { max.x = maxPos_world.x; }
                    if (maxPos_world.y > max.y) { max.y = maxPos_world.y; }
                    if (maxPos_world.z > max.z) { max.z = maxPos_world.z; }
                }
            }

            //如果这个物体不包含Mesh,即初始化的值一个都没有改动
            if (min.x == float.MaxValue)
            {
                Debug.LogWarning("BoundsExtensions.GetBoundsLocal():这个物体组下所有物体都不包含一个MeshFilter");
                return default(Bounds);
            }

            //世界坐标转回本地（使用物体0作为基准点）
            Vector3 center = gos[0].transform.worldToLocalMatrix.MultiplyPoint((max + min) * 0.5F);
            Vector3 size = max - min;

            gos[0].transform.rotation = recRotation;//恢复记录
            gos[0].transform.localScale = recScale;

            return new Bounds(center, size);
        }

        public static Bounds GetBoundsUnRotate(this GameObject[] gos)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            if (gos.Length == 0)
            {
                Debug.LogError("BoundsExtensions.GetBoundsLocal():输入参数错误!");
                return default(Bounds);
            }

            //这里只能先把物体放平，等会再放回来
            Quaternion recRotation = gos[0].transform.rotation;
            gos[0].transform.rotation = Quaternion.identity;
            //Vector3 recScale = gos[0].transform.localScale;
            //gos[0].transform.localScale = Vector3.one;

            for (int index = 0; index < gos.Length; index++)
            {
                GameObject go = gos[index];
                if (go == null)
                {
                    Debug.LogError("BoundsExtensions.GetBoundsLocal():输入参数的物体组下有的物体为null!");
                    return default(Bounds);
                }

                //得到所有MeshFilter
                MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
                //蒙皮上没有网格 直接引用模型的 并没有直接添加在模型上
                SkinnedMeshRenderer[] skins = go.GetComponentsInChildren<SkinnedMeshRenderer>();

                for (int i = 0; i < skins.Length; i++)
                {
                    Vector3 minPos_world = skins[i].transform.localToWorldMatrix.MultiplyPoint(skins[i].sharedMesh.bounds.min);
                    Vector3 maxPos_world = skins[i].transform.localToWorldMatrix.MultiplyPoint(skins[i].sharedMesh.bounds.max);
                    //使用minPos_world和maxPos_world两个点，比对更新min的值
                    if (minPos_world.x < min.x) { min.x = minPos_world.x; }
                    if (minPos_world.y < min.y) { min.y = minPos_world.y; }
                    if (minPos_world.z < min.z) { min.z = minPos_world.z; }
                    if (maxPos_world.x < min.x) { min.x = maxPos_world.x; }
                    if (maxPos_world.y < min.y) { min.y = maxPos_world.y; }
                    if (maxPos_world.z < min.z) { min.z = maxPos_world.z; }

                    //使用minPos_world和maxPos_world两个点，比对更新max的值
                    if (minPos_world.x > max.x) { max.x = minPos_world.x; }
                    if (minPos_world.y > max.y) { max.y = minPos_world.y; }
                    if (minPos_world.z > max.z) { max.z = minPos_world.z; }
                    if (maxPos_world.x > max.x) { max.x = maxPos_world.x; }
                    if (maxPos_world.y > max.y) { max.y = maxPos_world.y; }
                    if (maxPos_world.z > max.z) { max.z = maxPos_world.z; }
                }

                for (int i = 0; i < meshFilters.Length; i++)
                {
                    //获取每个模型最大的点和最小的点，然后变换到世界空间，为了筛选出模型边框的bounds
                    Vector3 minPos_world = meshFilters[i].transform.localToWorldMatrix.MultiplyPoint(meshFilters[i].mesh.bounds.min);
                    Vector3 maxPos_world = meshFilters[i].transform.localToWorldMatrix.MultiplyPoint(meshFilters[i].mesh.bounds.max);

                    //使用minPos_world和maxPos_world两个点，比对更新min的值
                    if (minPos_world.x < min.x) { min.x = minPos_world.x; }
                    if (minPos_world.y < min.y) { min.y = minPos_world.y; }
                    if (minPos_world.z < min.z) { min.z = minPos_world.z; }
                    if (maxPos_world.x < min.x) { min.x = maxPos_world.x; }
                    if (maxPos_world.y < min.y) { min.y = maxPos_world.y; }
                    if (maxPos_world.z < min.z) { min.z = maxPos_world.z; }

                    //使用minPos_world和maxPos_world两个点，比对更新max的值
                    if (minPos_world.x > max.x) { max.x = minPos_world.x; }
                    if (minPos_world.y > max.y) { max.y = minPos_world.y; }
                    if (minPos_world.z > max.z) { max.z = minPos_world.z; }
                    if (maxPos_world.x > max.x) { max.x = maxPos_world.x; }
                    if (maxPos_world.y > max.y) { max.y = maxPos_world.y; }
                    if (maxPos_world.z > max.z) { max.z = maxPos_world.z; }
                }
            }

            //如果这个物体不包含Mesh,即初始化的值一个都没有改动
            if (min.x == float.MaxValue)
            {
                Debug.LogWarning("BoundsExtensions.GetBoundsLocal():这个物体组下所有物体都不包含一个MeshFilter");
                return default(Bounds);
            }

            //世界坐标转回本地（使用物体0作为基准点）
            Vector3 center = gos[0].transform.worldToLocalMatrix.MultiplyPoint((max + min) * 0.5F);
            Vector3 size = max - min;

            gos[0].transform.rotation = recRotation;//恢复记录
            //gos[0].transform.localScale = recScale;

            return new Bounds(center, size);
        }


        public static float GetXLen(this Bounds bounds)
        {
            return bounds.extents.x * 2;
        }

        public static float GetYLen(this Bounds bounds)
        {
            return bounds.extents.y * 2;
        }

        public static float GetZLen(this Bounds bounds)
        {
            return bounds.extents.z * 2;
        }

        /// <summary>
        /// 从Bounds的6个顶点中去得到某一个边界点的坐标。分别对x,y,z输入1或者-1表示选择。
        /// 如果输入(1,1,1)表示得到一个顶点它的x,y,z都是最大的。
        /// 可以组合得到Bounds的8个顶点。
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="slectX">只能是1或者-1</param>
        /// <param name="slectY">只能是1或者-1</param>
        /// <param name="slectZ">只能是1或者-1</param>
        /// <returns></returns>
        public static Vector3 GetPonit(this Bounds bounds, int slectX, int slectY, int slectZ)
        {
            if ((slectX == 1 || slectX == -1) &&
                (slectY == 1 || slectY == -1) &&
                (slectZ == 1 || slectZ == -1))
            {
                Vector3 v = new Vector3(bounds.extents.x * slectX,
                                        bounds.extents.y * slectY,
                                        bounds.extents.z * slectZ);
                return bounds.center + v;
            }
            else
            {
                //非法的输入
                Debug.LogError("BoundsExtensions.GetPonit():slect参数必须为1或-1,错误的参数输入！");
                return default(Vector3);
            }
        }

        /// <summary>
        /// 得到一个Bounds的12条边,返回一个包含12条边的BoundSide数组<
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns>包含12条边的数组</returns>
        public static BoundSide[] GetSides(this Bounds bounds)
        {
            //正向搜索12条边
            BoundSide[] sides = new BoundSide[12];
            int curIndex = 0;
            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        if (x < 0)
                        {
                            sides[curIndex].p1 = bounds.GetPonit(x, y, z);
                            sides[curIndex].p2 = bounds.GetPonit(-x, y, z);
                            curIndex++;
                        }
                        if (y < 0)
                        {
                            sides[curIndex].p1 = bounds.GetPonit(x, y, z);
                            sides[curIndex].p2 = bounds.GetPonit(x, -y, z);
                            curIndex++;
                        }
                        if (z < 0)
                        {
                            sides[curIndex].p1 = bounds.GetPonit(x, y, z);
                            sides[curIndex].p2 = bounds.GetPonit(x, y, -z);
                            curIndex++;
                        }
                    }
                }
            }
            return sides;
        }

        /// <summary>
        /// 得到一个Bounds的12条边,返回一个包含12条边的BoundSide数组<
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns>包含12条边的数组</returns>
        public static void GetSides(this Bounds bounds, BoundSide[] sides)
        {
            if (sides.Length != 12)
            {
                Debug.LogError("BoundsExtensions.GetSides():sides长度必须为12 ,错误的参数输入！");
                return;
            }

            //正向搜索12条边
            int curIndex = 0;
            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        if (x < 0)
                        {
                            sides[curIndex].p1 = bounds.GetPonit(x, y, z);
                            sides[curIndex].p2 = bounds.GetPonit(-x, y, z);
                            curIndex++;
                        }
                        if (y < 0)
                        {
                            sides[curIndex].p1 = bounds.GetPonit(x, y, z);
                            sides[curIndex].p2 = bounds.GetPonit(x, -y, z);
                            curIndex++;
                        }
                        if (z < 0)
                        {
                            sides[curIndex].p1 = bounds.GetPonit(x, y, z);
                            sides[curIndex].p2 = bounds.GetPonit(x, y, -z);
                            curIndex++;
                        }
                    }
                }
            }
        }
    }
}