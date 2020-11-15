using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UniHumanoid
{
    public interface ISkeletonDetector
    {
        Skeleton Detect(IList<IBone> bones);
    }


    public class BvhSkeletonEstimator : ISkeletonDetector
    {
        static IBone GetRoot(IList<IBone> bones)
        {
            var hips = bones.Where(x => x.Parent == null).ToArray();
            if (hips.Length != 1)
            {
                throw new System.Exception("Require unique root");
            }
            return hips[0];
        }

        public Skeleton Detect(IList<IBone> bones)
        {
            //
            // search bones
            //
            var skeleton = new Skeleton();
            var root = GetRoot(bones);
            if(root.Name == "全ての親")
            {
                skeleton.Set(HumanBodyBones.Hips, bones, root);
            }
            foreach (var child in root.Children)
            {
                foreach (var x in child.Traverse())
                { 
                    switch (x.Name)
                    {
                        case "全ての親":
                            skeleton.Set(HumanBodyBones.Hips, bones, x);
                            break;
                        case "センター":
                            skeleton.Set(HumanBodyBones.Spine, bones, x);
                            break;
                        case "上半身":
                            skeleton.Set(HumanBodyBones.Chest, bones, x);
                            break;
                        case "上半身2":
                            skeleton.Set(HumanBodyBones.UpperChest, bones, x);
                            break;
                        case "頭":
                            skeleton.Set(HumanBodyBones.Head, bones, x);
                            break;
                        case "首":
                            skeleton.Set(HumanBodyBones.Neck, bones, x);
                            break;

                        case "左足":
                            skeleton.Set(HumanBodyBones.LeftUpperLeg, bones, x);
                            break;
                        case "左ひざ":
                            skeleton.Set(HumanBodyBones.LeftLowerLeg, bones, x);
                            break;
                        case "左足首":
                            skeleton.Set(HumanBodyBones.LeftFoot, bones, x);
                            break;
                        case "左つま先":
                            skeleton.Set(HumanBodyBones.LeftToes, bones, x);
                            break;

                        case "右足":
                            skeleton.Set(HumanBodyBones.RightUpperLeg, bones, x);
                            break;
                        case "右ひざ":
                            skeleton.Set(HumanBodyBones.RightLowerLeg, bones, x);
                            break;
                        case "右足首":
                            skeleton.Set(HumanBodyBones.RightFoot, bones, x);
                            break;
                        case "右つま先":
                            skeleton.Set(HumanBodyBones.RightToes, bones, x);
                            break;

                        case "左肩":
                            skeleton.Set(HumanBodyBones.LeftShoulder, bones, x);
                            break;
                        case "左腕":
                            skeleton.Set(HumanBodyBones.LeftUpperArm, bones, x);
                            break;
                        case "左ひじ":
                            skeleton.Set(HumanBodyBones.LeftLowerArm, bones, x);
                            break;
                        case "左手首":
                            skeleton.Set(HumanBodyBones.LeftHand, bones, x);
                            break;

                        case "右肩":
                            skeleton.Set(HumanBodyBones.RightShoulder, bones, x);
                            break;
                        case "右腕":
                            skeleton.Set(HumanBodyBones.RightUpperArm, bones, x);
                            break;
                        case "右ひじ":
                            skeleton.Set(HumanBodyBones.RightLowerArm, bones, x);
                            break;
                        case "右手首":
                            skeleton.Set(HumanBodyBones.RightHand, bones, x);
                            break;

                        case "右親指０":
                            skeleton.Set(HumanBodyBones.RightThumbProximal, bones, x);
                            break;
                        case "右親指１":
                            skeleton.Set(HumanBodyBones.RightThumbIntermediate, bones, x);
                            break;
                        case "右親指２":
                            skeleton.Set(HumanBodyBones.RightThumbDistal, bones, x);
                            break;
                        
                        case "右人指１":
                            skeleton.Set(HumanBodyBones.RightIndexProximal, bones, x);
                            break;
                        case "右人指２":
                            skeleton.Set(HumanBodyBones.RightIndexIntermediate, bones, x);
                            break;
                        case "右人指３":
                            skeleton.Set(HumanBodyBones.RightIndexDistal, bones, x);
                            break;

                        case "右中指１":
                            skeleton.Set(HumanBodyBones.RightMiddleProximal, bones, x);
                            break;
                        case "右中指２":
                            skeleton.Set(HumanBodyBones.RightMiddleIntermediate, bones, x);
                            break;
                        case "右中指３":
                            skeleton.Set(HumanBodyBones.RightMiddleDistal, bones, x);
                            break;

                        case "右薬指１":
                            skeleton.Set(HumanBodyBones.RightRingProximal, bones, x);
                            break;
                        case "右薬指２":
                            skeleton.Set(HumanBodyBones.RightRingIntermediate, bones, x);
                            break;
                        case "右薬指３":
                            skeleton.Set(HumanBodyBones.RightRingDistal, bones, x);
                            break;

                        case "右小指１":
                            skeleton.Set(HumanBodyBones.RightLittleProximal, bones, x);
                            break;
                        case "右小指２":
                            skeleton.Set(HumanBodyBones.RightLittleIntermediate, bones, x);
                            break;
                        case "右小指３":
                            skeleton.Set(HumanBodyBones.RightLittleDistal, bones, x);
                            break;

                        case "左親指０":
                            skeleton.Set(HumanBodyBones.LeftThumbProximal, bones, x);
                            break;
                        case "左親指１":
                            skeleton.Set(HumanBodyBones.LeftThumbIntermediate, bones, x);
                            break;
                        case "左親指２":
                            skeleton.Set(HumanBodyBones.LeftThumbDistal, bones, x);
                            break;

                        case "左人指１":
                            skeleton.Set(HumanBodyBones.LeftIndexProximal, bones, x);
                            break;
                        case "左人指２":
                            skeleton.Set(HumanBodyBones.LeftIndexIntermediate, bones, x);
                            break;
                        case "左人指３":
                            skeleton.Set(HumanBodyBones.LeftIndexDistal, bones, x);
                            break;

                        case "左中指１":
                            skeleton.Set(HumanBodyBones.LeftMiddleProximal, bones, x);
                            break;
                        case "左中指２":
                            skeleton.Set(HumanBodyBones.LeftMiddleIntermediate, bones, x);
                            break;
                        case "左中指３":
                            skeleton.Set(HumanBodyBones.LeftMiddleDistal, bones, x);
                            break;

                        case "左薬指１":
                            skeleton.Set(HumanBodyBones.LeftRingProximal, bones, x);
                            break;
                        case "左薬指２":
                            skeleton.Set(HumanBodyBones.LeftRingIntermediate, bones, x);
                            break;
                        case "左薬指３":
                            skeleton.Set(HumanBodyBones.LeftRingDistal, bones, x);
                            break;

                        case "左小指１":
                            skeleton.Set(HumanBodyBones.LeftLittleProximal, bones, x);
                            break;
                        case "左小指２":
                            skeleton.Set(HumanBodyBones.LeftLittleIntermediate, bones, x);
                            break;
                        case "左小指３":
                            skeleton.Set(HumanBodyBones.LeftLittleDistal, bones, x);
                            break;
                    }
                }
            }
            return skeleton;
        }

        public Skeleton Detect(Bvh bvh)
        {
            var root = new BvhBone(bvh.Root.Name, Vector3.zero);
            root.Build(bvh.Root);
            return Detect(root.Traverse().Select(x => (IBone)x).ToList());
        }

        public Skeleton Detect(Transform t)
        {
            var root = new BvhBone(t.name, Vector3.zero);
            root.Build(t);
            return Detect(root.Traverse().Select(x => (IBone)x).ToList());
        }
    }
}
