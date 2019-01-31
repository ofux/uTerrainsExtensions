using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Ultimate Terrains/Player builder")]
public class PlayerBuilder : MonoBehaviour
{
    private const string SphereName = "Sphere",
                         SmoothName = "Smooth",
                         SharpenName = "Sharpen",
                         PaintName = "Paint",
                         AdditiveSphereName = "Additive Sphere",
                         AxisAlignedCubeName = "Axis-aligned Cube",
                         CapsuleName = "Capsule",
                         CylinderName = "Cylinder",
                         ParallelepipedName = "Parallelepiped";

    public UltimateTerrain Terrain;
    public Dropdown operationTypeDropdown;
    public Dropdown voxelTypeDropdown;
    public Toggle nonStopEditingToggle;
    public Toggle followGridToggle;
    public Toggle asyncOperationsToggle;
    public Slider brushSizeSlider;
    public Text targetVoxelText;
    public Toggle digToggle;
    public float MaxModifyDistance = 500f;
    public float BrushSize = 1f;
    public bool PreventPlayerMovesUntilLoaded;
    public bool KeepPlayerAboveGround;

    private readonly List<string> operationTypes = new List<string> {SphereName, PaintName, AdditiveSphereName, SmoothName, SharpenName, AxisAlignedCubeName, CapsuleName, CylinderName, ParallelepipedName};

    private Vector3 initialPlayerPos;
    private VoxelType voxelType;
    private string operationName;
    private bool isNonStopEditing;
    private bool followGrid;
    private bool asyncOperations;

    private ReticleForEditor reticleCubical;
    private SphericalReticleForEditor reticleSpherical;
    private ReticleLinesForEditor reticleLinesForEditor;
    private CapsuleReticleForEditor capsuleReticleForEditor;
    private CylinderReticleForEditor cylinderReticleForEditor;

    private bool dig;
    private bool clicking;
    private Vector3? firstClickPosition;

    // Use this for initialization
    private void Start()
    {
        if (!Terrain) {
            Debug.LogError("PlayerBuilder -> Terrain is undefined. Please drag the uTerrain object into the 'Player' field of PlayerBuilder.");
            enabled = false;
            return;
        }

        initialPlayerPos = transform.position;
        UltimateTerrain.OnLoaded += OnTerrainLoaded;

        LoadReticles();

        operationName = SphereName;

        operationTypeDropdown.ClearOptions();
        operationTypeDropdown.AddOptions(operationTypes);
        operationTypeDropdown.onValueChanged.AddListener(delegate { OnDropdownOperationTypeChanged(operationTypeDropdown); });


        voxelTypeDropdown.ClearOptions();
        voxelTypeDropdown.AddOptions(new List<string>(Terrain.VoxelTypeSet.VoxelTypeNames));
        voxelTypeDropdown.onValueChanged.AddListener(delegate { OnDropdownVoxelTypeChanged(voxelTypeDropdown); });

        nonStopEditingToggle.onValueChanged.AddListener(delegate { OnToggleNonStopEditingChanged(nonStopEditingToggle); });

        followGridToggle.onValueChanged.AddListener(delegate { OnToggleFollowGridChanged(followGridToggle); });

        asyncOperationsToggle.onValueChanged.AddListener(delegate { OnToggleAsyncOperationsChanged(asyncOperationsToggle); });

        brushSizeSlider.onValueChanged.AddListener(delegate { OnSliderBrushSizeChanged(brushSizeSlider); });

        digToggle.onValueChanged.AddListener(delegate { OnToggleDigChanged(digToggle); });

        followGrid = followGridToggle.isOn;
        isNonStopEditing = nonStopEditingToggle.isOn;
        asyncOperations = asyncOperationsToggle.isOn;
        BrushSize = brushSizeSlider.value;
        targetVoxelText.text = "";
        dig = digToggle.isOn;
    }

    private void LoadReticles()
    {
        var reticlePrefab = Resources.Load("ReticleForEditor") as GameObject;
        var retAddObj = Instantiate(reticlePrefab, Vector3.zero, Quaternion.identity);
        reticleCubical = retAddObj.GetComponent<ReticleForEditor>();
        reticleCubical.Initialize();
        reticleCubical.EnableRenderer(false);

        reticlePrefab = Resources.Load("SphericalReticleForEditor") as GameObject;
        retAddObj = Instantiate(reticlePrefab, Vector3.zero, Quaternion.identity);
        reticleSpherical = retAddObj.GetComponent<SphericalReticleForEditor>();
        reticleSpherical.Initialize();
        reticleSpherical.EnableRenderer(false);

        reticlePrefab = Resources.Load("ReticleLinesForEditor") as GameObject;
        retAddObj = Instantiate(reticlePrefab, Vector3.zero, Quaternion.identity);
        reticleLinesForEditor = retAddObj.GetComponent<ReticleLinesForEditor>();
        reticleLinesForEditor.Initialize();
        reticleLinesForEditor.EnableRenderer(false);

        reticlePrefab = Resources.Load("CapsuleReticleForEditor") as GameObject;
        retAddObj = Instantiate(reticlePrefab, Vector3.zero, Quaternion.identity);
        capsuleReticleForEditor = retAddObj.GetComponent<CapsuleReticleForEditor>();
        capsuleReticleForEditor.Initialize();
        capsuleReticleForEditor.EnableRenderer(false);

        reticlePrefab = Resources.Load("CylinderReticleForEditor") as GameObject;
        retAddObj = Instantiate(reticlePrefab, Vector3.zero, Quaternion.identity);
        cylinderReticleForEditor = retAddObj.GetComponent<CylinderReticleForEditor>();
        cylinderReticleForEditor.Initialize();
        cylinderReticleForEditor.EnableRenderer(false);
    }

    private void HideAllReticles()
    {
        reticleCubical.EnableRenderer(false);
        reticleSpherical.EnableRenderer(false);
        reticleLinesForEditor.EnableRenderer(false);
        capsuleReticleForEditor.EnableRenderer(false);
        cylinderReticleForEditor.EnableRenderer(false);
    }


    public void OnDropdownOperationTypeChanged(Dropdown change)
    {
        operationName = change.captionText.text;
        HideAllReticles();
        firstClickPosition = null;
    }

    public void OnDropdownVoxelTypeChanged(Dropdown change)
    {
        voxelType = Terrain.VoxelTypeSet.GetVoxelType(change.captionText.text);
    }

    public void OnToggleFollowGridChanged(Toggle change)
    {
        followGrid = change.isOn;
    }

    public void OnToggleAsyncOperationsChanged(Toggle change)
    {
        asyncOperations = change.isOn;
    }

    public void OnToggleNonStopEditingChanged(Toggle change)
    {
        isNonStopEditing = change.isOn;
    }

    public void OnSliderBrushSizeChanged(Slider change)
    {
        BrushSize = change.value;
    }

    public void OnToggleDigChanged(Toggle change)
    {
        dig = change.isOn;
    }

    // Update is called once per frame
    private void Update()
    {
        if (PreventPlayerMovesUntilLoaded) {
            PreventPlayerMoves();
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? Cursor.lockState = CursorLockMode.None : Cursor.lockState = CursorLockMode.Locked;
        }

        if (!Terrain.IsLoaded || voxelType == null || string.IsNullOrEmpty(operationName))
            return;

        var intersection = GetIntersectionWithTerrain(dig);
        if (intersection.HasValue) {
            var wpos = intersection.Value.point + intersection.Value.normal * 0.01f;
            if (followGrid) {
                wpos = GetFollowGrid(wpos);
            }

            if (!Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0)) {
                clicking = true;
            } else if (!isNonStopEditing || Input.GetMouseButtonUp(0)) {
                clicking = false;
            }

            switch (operationName) {
                case AxisAlignedCubeName:
                    UpdateForAxisAlignedCube(wpos);
                    break;
                case SphereName:
                    UpdateForSphere(wpos);
                    break;
                case AdditiveSphereName:
                    UpdateForAdditiveSphere(wpos);
                    break;
                case PaintName:
                    UpdateForPaint(wpos);
                    break;
                case ParallelepipedName:
                    DisableContinuousEditing();
                    UpdateForParallelepiped(wpos);
                    break;
                case CylinderName:
                    DisableContinuousEditing();
                    UpdateForCylinder(wpos);
                    break;
                case CapsuleName:
                    DisableContinuousEditing();
                    UpdateForCapsule(wpos);
                    break;
                case SmoothName:
                    DisableContinuousEditing();
                    UpdateForSmooth(wpos);
                    break;
                case SharpenName:
                    DisableContinuousEditing();
                    UpdateForSharpen(wpos);
                    break;
            }

            if (Input.GetMouseButtonDown(1)) {
                var targetVoxel = Terrain.GetVoxelAt(Terrain.Converter.UnityToVoxelPositionFloor(wpos));
                targetVoxelText.text = targetVoxel.ToString();
            }
        }

        if (KeepPlayerAboveGround) {
            KeepPlayerAbove();
        }
    }

    private void DisableContinuousEditing()
    {
        isNonStopEditing = false;
        nonStopEditingToggle.isOn = false;
    }

    private void UpdateForAxisAlignedCube(Vector3 wpos)
    {
        reticleCubical.EnableRenderer(true);
        reticleCubical.SetPositionAndSize(wpos, Vector3.one * BrushSize);

        if (clicking && Terrain.OperationsManager.IsReadyToComputeAsync) {
            Terrain.OperationsManager
                   .Add(AxisAlignedCube.CreateFromUnityWorld(Terrain, dig, wpos, BrushSize * Vector3.one, voxelType), true)
                   .PerformAll(asyncOperations);
        }
    }

    private void UpdateForSphere(Vector3 wpos)
    {
        reticleSpherical.EnableRenderer(true);
        reticleSpherical.SetPositionAndSize(wpos, BrushSize);

        if (clicking && Terrain.OperationsManager.IsReadyToComputeAsync) {
            Terrain.OperationsManager
                   .Add(Sphere.CreateFromUnityWorld(Terrain, dig, wpos, BrushSize, voxelType), true)
                   .PerformAll(asyncOperations);
        }
    }

    private void UpdateForAdditiveSphere(Vector3 wpos)
    {
        reticleSpherical.EnableRenderer(true);
        reticleSpherical.SetPositionAndSize(wpos, BrushSize);

        if (clicking && Terrain.OperationsManager.IsReadyToComputeAsync) {
            Terrain.OperationsManager
                   .Add(AdditiveSphere.CreateFromUnityWorld(Terrain, dig, wpos, BrushSize, 0.5, voxelType), true)
                   .PerformAll(asyncOperations);
        }
    }

    private void UpdateForPaint(Vector3 wpos)
    {
        reticleSpherical.EnableRenderer(true);
        reticleSpherical.SetPositionAndSize(wpos, BrushSize);

        if (clicking && Terrain.OperationsManager.IsReadyToComputeAsync) {
            Terrain.OperationsManager
                   .Add(Paint.CreateFromUnityWorld(Terrain, wpos, BrushSize, voxelType), true)
                   .PerformAll(asyncOperations);
        }
    }

    private void UpdateForSmooth(Vector3 wpos)
    {
        reticleSpherical.EnableRenderer(true);
        reticleSpherical.SetPositionAndSize(wpos, BrushSize);

        if (clicking && Terrain.OperationsManager.IsReadyToComputeAsync) {
            Terrain.OperationsManager
                   .Add(Smooth.CreateFromUnityWorld(Terrain, wpos, BrushSize, Math.Max(1.0, BrushSize / 3.0)), true)
                   .PerformAll(asyncOperations);
        }
    }

    private void UpdateForSharpen(Vector3 wpos)
    {
        reticleSpherical.EnableRenderer(true);
        reticleSpherical.SetPositionAndSize(wpos, BrushSize);

        if (clicking && Terrain.OperationsManager.IsReadyToComputeAsync) {
            Terrain.OperationsManager
                   .Add(Sharpen.CreateFromUnityWorld(Terrain, wpos, BrushSize, Math.Max(1.0, BrushSize / 3.0)), true)
                   .PerformAll(asyncOperations);
        }
    }

    private void UpdateForParallelepiped(Vector3 wpos)
    {
        reticleLinesForEditor.EnableRenderer(true);

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButtonDown(1)) {
            firstClickPosition = null;
        }

        var p = wpos - new Vector3(0, BrushSize * 0.5f, BrushSize * 0.5f);
        Vector3 vL, vH, vW;
        if (firstClickPosition.HasValue) {
            UMath.ComputeEdgeVectors(firstClickPosition.Value, p, BrushSize, BrushSize, out vL, out vH, out vW);
            reticleLinesForEditor.SetCornerAndEdges(firstClickPosition.Value, vL, vH, vW);
        } else {
            UMath.ComputeEdgeVectors(p, p + Vector3.forward, BrushSize, BrushSize, out vL, out vH, out vW);
            reticleLinesForEditor.SetCornerAndEdges(p, vL, vH, vW);
        }

        if (clicking && Terrain.OperationsManager.IsReadyToComputeAsync) {
            if (!firstClickPosition.HasValue) {
                firstClickPosition = p;
            } else {
                Vector3 vLd, vHd, vWd;
                UMath.ComputeEdgeVectors(
                    firstClickPosition.Value, p, BrushSize, BrushSize, out vLd, out vHd, out vWd);
                Terrain.OperationsManager
                       .Add(Parallelepiped.CreateFromUnityWorld(Terrain, dig, firstClickPosition.Value, vLd, vHd, vWd, voxelType), true)
                       .PerformAll(asyncOperations);
                firstClickPosition = null;
            }
        }
    }

    private void UpdateForCylinder(Vector3 wpos)
    {
        cylinderReticleForEditor.EnableRenderer(true);

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButtonDown(1)) {
            firstClickPosition = null;
        }

        if (firstClickPosition.HasValue) {
            cylinderReticleForEditor.SetPositionsAndRadius(firstClickPosition.Value, wpos, BrushSize);
        } else {
            cylinderReticleForEditor.SetPositionsAndRadius(wpos, wpos + Vector3.right, BrushSize);
        }

        if (clicking && Terrain.OperationsManager.IsReadyToComputeAsync) {
            if (!firstClickPosition.HasValue) {
                firstClickPosition = wpos;
            } else {
                Terrain.OperationsManager
                       .Add(Cylinder.CreateFromUnityWorld(Terrain, dig, firstClickPosition.Value, wpos, BrushSize, voxelType), true)
                       .PerformAll(asyncOperations);
                firstClickPosition = null;
            }
        }
    }

    private void UpdateForCapsule(Vector3 wpos)
    {
        capsuleReticleForEditor.EnableRenderer(true);

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButtonDown(1)) {
            firstClickPosition = null;
        }

        if (firstClickPosition.HasValue) {
            capsuleReticleForEditor.SetPositionsAndRadius(firstClickPosition.Value, wpos, BrushSize);
        } else {
            capsuleReticleForEditor.SetPositionsAndRadius(wpos, wpos + Vector3.right, BrushSize);
        }

        if (clicking && Terrain.OperationsManager.IsReadyToComputeAsync) {
            if (!firstClickPosition.HasValue) {
                firstClickPosition = wpos;
            } else {
                Terrain.OperationsManager
                       .Add(Capsule.CreateFromUnityWorld(Terrain, dig, firstClickPosition.Value, wpos, BrushSize, voxelType), true)
                       .PerformAll(asyncOperations);
                firstClickPosition = null;
            }
        }
    }

    private void PreventPlayerMoves()
    {
        transform.position = initialPlayerPos;
    }

    private void OnTerrainLoaded(UltimateTerrain sender)
    {
        PreventPlayerMovesUntilLoaded = false;
        if (Terrain.VoxelTypeSet.VoxelTypeNames != null && Terrain.VoxelTypeSet.VoxelTypeNames.Length > 0) {
            voxelType = Terrain.VoxelTypeSet.GetVoxelType(Terrain.VoxelTypeSet.VoxelTypeNames[0]);
        }
    }

    private RaycastHit? GetIntersectionWithTerrain(bool isDigging)
    {
        var offset = Camera.main.transform.forward;
        if (!isDigging) {
            // prevent building over the player
            offset = offset * BrushSize;
        }

        var ray = new Ray(Camera.main.transform.position + offset, Camera.main.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, MaxModifyDistance, Terrain.Params.ChunkLayerMask)) {
            if (isDigging && BrushSize < 2f) {
                hit.point = hit.point + ray.direction * Terrain.Params.SizeXVoxelF * 0.5f;
            }
        }

        return hit;
    }

    private void KeepPlayerAbove()
    {
        var ray = new Ray(Camera.main.transform.position, Vector3.down);

        RaycastHit hit;
        if (!Physics.Raycast(ray.origin, ray.direction, out hit, 1000f, Terrain.Params.ChunkLayerMask)) {
            if (Physics.Raycast(ray.origin + Vector3.up * 999f, ray.direction, out hit, 1000f, Terrain.Params.ChunkLayerMask)) {
                transform.position = hit.point + Vector3.up * 3f;
            }
        }
    }

    private Vector3 GetFollowGrid(Vector3 worldPosition)
    {
        return (Vector3) Terrain.Converter.VoxelToUnityPosition(Terrain.Converter.UnityToVoxelPositionRound(worldPosition));
    }
}