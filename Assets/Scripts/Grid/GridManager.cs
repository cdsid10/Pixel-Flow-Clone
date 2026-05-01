using System.Collections.Generic;
using Scripts.Core;
using Scripts.Enums;
using Scripts.Misc;
using UnityEngine;

namespace Scripts.Grid
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [SerializeField]
        private int width = 10;
        public int Width => width;

        [SerializeField]
        private int height = 10;
        public int Height => height;

        [SerializeField]
        private float cubeSpacing = 1f;

        [SerializeField]
        private float spawnOffset = 0.5f;

        public GridCubeView cubePrefab;

        public GridCube[,] gridCubeArray;

        public GridCubeView[,] gridCubeViewArray;

        public Dictionary<ColorTypeEnum, int> colorTypeCountDictionary = new();

        public Dictionary<ColorTypeEnum, ColorTypeSO> colorTypeDataDictionary = new();

        public List<ColorTypeSO> colorDataList;

        public List<ColorTypeEnum> colorTypePoolList = new();

        [SerializeField]
        private List<GridCubeView> currentlyLeftCubeViews = new();

        [SerializeField] private bool useManualLevelLayout;

        [SerializeField] private LevelDataSO levelDataSO;

        void OnValidate()
        {
            if (useManualLevelLayout && levelDataSO != null)
            {
                if (levelDataSO.width < 1) levelDataSO.width = 1;
                if (levelDataSO.height < 1) levelDataSO.height = 1;

                while (levelDataSO.cubeRows.Count < levelDataSO.height)
                {
                    levelDataSO.cubeRows.Add(new LevelRowData());
                }

                while (levelDataSO.cubeRows.Count > levelDataSO.height)
                {
                    levelDataSO.cubeRows.RemoveAt(levelDataSO.cubeRows.Count - 1);
                }

                for (int y = 0; y < levelDataSO.cubeRows.Count; y++)
                {
                    if (levelDataSO.cubeRows[y] == null)
                    {
                        levelDataSO.cubeRows[y] = new LevelRowData();
                    }

                    while (levelDataSO.cubeRows[y].cubeColumns.Count < levelDataSO.width)
                    {
                        levelDataSO.cubeRows[y].cubeColumns.Add(ColorTypeEnum.None);
                    }

                    while (levelDataSO.cubeRows[y].cubeColumns.Count > levelDataSO.width)
                    {
                        levelDataSO.cubeRows[y].cubeColumns.RemoveAt(levelDataSO.cubeRows[y].cubeColumns.Count - 1);
                    }
                }
            }
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            InitData();

            if (useManualLevelLayout && levelDataSO != null)
            {
                BuildLevelData();
            }
            else
            {
                DecideColorCountsForCubes();
            }

            GenerateGrid();
        }

        private void InitData()
        {
            colorTypeCountDictionary.Clear();
            colorTypeDataDictionary.Clear();

            foreach (var color in colorDataList)
            {
                colorTypeCountDictionary[color.colorType] = 0;
                colorTypeDataDictionary[color.colorType] = color;
            }
        }

        public void GenerateGrid()
        {
            if (useManualLevelLayout && levelDataSO != null)
            {
                gridCubeArray = new GridCube[levelDataSO.width, levelDataSO.height];
                gridCubeViewArray = new GridCubeView[levelDataSO.width, levelDataSO.height];
            }
            else
            {
                gridCubeArray = new GridCube[width, height];
                gridCubeViewArray = new GridCubeView[width, height];
            }

            SpawnGridCubes();
        }

        public void DecideColorCountsForCubes()
        {
            var totalCubes = height * width;
            var colorCount = colorDataList.Count;

            var baseCount = totalCubes / colorCount;
            var remainingCubes = totalCubes % colorCount;

            colorTypePoolList.Clear();

            var colorTypeBaseCountDictionary = new Dictionary<ColorTypeEnum, int>();

            //distrubute base count for each color type
            foreach (var color in colorDataList)
            {
                colorTypeBaseCountDictionary[color.colorType] = baseCount;
            }

            //distribute remaining cubes randomly to color types
            for (int i = 0; i < remainingCubes; i++)
            {
                var randomColorDataIndex = UnityEngine.Random.Range(0, colorDataList.Count);
                var randomColorType = colorDataList[randomColorDataIndex].colorType;
                colorTypeBaseCountDictionary[randomColorType]++;
            }

            foreach (var colorKP in colorTypeBaseCountDictionary)
            {
                for (int i = 0; i < colorKP.Value; i++)
                {
                    colorTypePoolList.Add(colorKP.Key);
                }
            }

            //Debug.Log($"Color type pool list count: {colorTypePoolList.Count}");

            ShufflePool(colorTypePoolList);
        }

        private void BuildLevelData()
        {
            colorTypePoolList.Clear();

            for (int x = 0; x < levelDataSO.width; x++)
            {
                for (int y = 0; y < levelDataSO.height; y++)
                {
                    colorTypePoolList.Add(levelDataSO.cubeRows[y].cubeColumns[x]);
                }
            }
        }

        public void SpawnGridCubes()
        {
            var index = 0;

            if (useManualLevelLayout && levelDataSO != null)
            {
                for (int x = 0; x < levelDataSO.width; x++)
                {
                    for (int y = 0; y < levelDataSO.height; y++)
                    {
                        gridCubeArray[x, y] = new GridCube(colorTypePoolList[index]);
                        AdjustColorCountDictionary(colorTypePoolList[index], 1);
                        SpawnGridCubeView(x, y, colorTypePoolList[index]);
                        index++;
                    }
                }
            }
            else
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        gridCubeArray[x, y] = new GridCube(colorTypePoolList[index]);
                        AdjustColorCountDictionary(colorTypePoolList[index], 1);
                        SpawnGridCubeView(x, y, colorTypePoolList[index]);
                        index++;
                    }
                }
            }
        }

        private void SpawnGridCubeView(int x, int y, ColorTypeEnum colorType)
        {
            Vector3 cubeSpawnPos = new Vector3(x + spawnOffset * cubeSpacing, 0, y + spawnOffset * cubeSpacing);
            var cube = Instantiate(cubePrefab, cubeSpawnPos, Quaternion.identity);
            cube.name = $"Cube_{x}_{y}_{colorType}";
            gridCubeViewArray[x, y] = cube;
            cube.SetGridCubeData(gridCubeArray[x, y]);
            cube.SetMaterialColor(colorTypeDataDictionary[colorType].color);
            currentlyLeftCubeViews.Add(cube);
        }

        // private ColorTypeEnum GetRandomColorData()
        // {
        //     int randomColorDataIndex = UnityEngine.Random.Range(0, colorDataList.Count);
        //     ColorTypeEnum randomColorTypeEnum = colorDataList[randomColorDataIndex].colorType;
        //     return randomColorTypeEnum;
        // }

        private void AdjustColorCountDictionary(ColorTypeEnum colorType, int deltaChange)
        {
            if (colorTypeCountDictionary.ContainsKey(colorType))
            {
                colorTypeCountDictionary[colorType] += deltaChange;
                //Debug.Log($"Updated count of {colorType} cubes: {colorTypeCountDictionary[colorType]}");
            }
        }

        public GridCube GetCubeAtGridPosition(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return gridCubeArray[x, y];
            }
            return null;
        }

        public GridCubeView GetCubeViewAtGridPosition(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return gridCubeViewArray[x, y];
            }
            return null;
        }

        public (int x, int y) TryGetCubeByColor(ColorTypeEnum colorTypeEnum)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cube = GetCubeAtGridPosition(x, y);
                    if (cube != null && !cube.isDestroyed && cube.colorType == colorTypeEnum)
                    {
                        // Debug.Log($"Found a {colorTypeEnum} cube at position ({x}, {y})");
                        return (x, y);
                    }
                }
            }
            return (-1, -1);
        }

        public void DestroyCubeAtGridPosition(int x, int y)
        {
            GridCube cube = GetCubeAtGridPosition(x, y);

            if (cube == null || cube.isDestroyed)
            {
                return;
            }

            cube.isDestroyed = true;
            AdjustColorCountDictionary(cube.colorType, -1);

            GridCubeView gridCubeView = GetCubeViewAtGridPosition(x, y);

            if (gridCubeView != null)
            {
                gridCubeView.DisableCubeView();
            }
        }

        public void DestroyCubeView(GridCubeView gridCubeView)
        {
            if (gridCubeView == null || gridCubeView.gridCubeData.isDestroyed)
            {
                return;
            }

            gridCubeView.gridCubeData.isDestroyed = true;
            gridCubeView.DisableCubeView();
            AdjustColorCountDictionary(gridCubeView.gridCubeData.colorType, -1);
            currentlyLeftCubeViews.Remove(gridCubeView);
            EffectsManager.Instance.PlayBlastEffect(gridCubeView.transform.position, gridCubeView.GetMaterialColor());

            if (currentlyLeftCubeViews.Count == 0)
            {
                GameManager.Instance.GameOver(true);
            }
        }




        private void ShufflePool(List<ColorTypeEnum> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);

                var temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}