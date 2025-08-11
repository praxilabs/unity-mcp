// using LiquidVolumeFX;
// using System.Collections;
// using UnityEngine;

// public class SpillController : SpillControllerBase
// {
//     public float particleDestroyTime = 1f;
//     public float minCapacity = 0;
//     private LiquidVolumeController liquidVolumeController = new LiquidVolumeController();

//     protected override void Start()
//     {
//         if (GetComponentInParent<LiquidVolumeController>() != null)
//             liquidVolumeController = GetComponentInParent<LiquidVolumeController>();

//         base.Start();
//     }

//     protected override void Update() {}

//     // Update is called once per frame
//     protected override void FixedUpdate()
//     {
//         if (minCapacity != 0)
//             if (lv.level * liquidVolumeController.containerCapacity <= minCapacity) return;

//         base.FixedUpdate();
//     }
//     protected override IEnumerator DestroySpill(GameObject spill)
//     {
//         yield return new WaitForSeconds(particleDestroyTime);
//         Destroy(spill);
//     }
// }