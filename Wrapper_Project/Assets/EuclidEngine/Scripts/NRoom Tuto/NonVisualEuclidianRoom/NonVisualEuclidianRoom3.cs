using System;
using System.Collections.Generic;
using UnityEngine;

/*
namespace EuclidEngine
{
    [AddComponentMenu("Euclid Engine/Non Euclidian Chamber")]
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody), typeof(Material))]


    [System.Serializable]
    public class NonVisualEuclidianRoom3: Area2
    {
        /* Beautiful print for editor */
        /*public static int Y = 0;
        public Vector3 pos;
        private int conf = 1;
        private int static_conf = 1;

        // 6  because that is the number of sides
        [SerializeField] public bool[][,] ArrayOfSides = new bool[6][,] { new bool[0, 0], new bool[0, 0], new bool[0, 0], new bool[0, 0], new bool[0, 0], new bool[0, 0] };
        public void changeY(int new_y)
        {
            Y = new_y;
            //boolArray2D = new bool[new_y, new_y];

            for (int i = 0; i < 6; ++i)
            {
                ArrayOfSides[i] = new bool[new_y, new_y];
            }

        }
        /* End of beautiful Editor */

        /*public int getConfig()
        {
            return conf;
        }

        /* Const var*/
        /*private const int NUMBER_OF_FACES = 6;
        string[] walls_name = { "BackSide", "FrontSide", "LeftSide", "RightSide", "TopSide", "BottomSide" };

        public void ChangeArraySize(ref bool[,] array, int size)
        {
            array = new bool[size, size];
        }

        /* Check Array */
        /*public List<ListableClass> BackSide = new List<ListableClass>();
        public List<ListableClass> FrontSide = new List<ListableClass>();
        public List<ListableClass> LeftSide = new List<ListableClass>();
        public List<ListableClass> RightSide = new List<ListableClass>();
        public List<ListableClass> TopSide = new List<ListableClass>();
        public List<ListableClass> BottomSide = new List<ListableClass>();

        private List<ListableClass>[] walls;
        private bool[] good_faces;

        /* Mutable var */
        /*private int StaticNumberOfPieces;
        [SerializeField] public int NumberOfPieces = 1;
        [SerializeField] public int EditorNumberOfPieces = 1;
        public GameObject myPrefab;

        public Material WallMaterial;
        private Material StaticWallMaterial = null;
        public float WallThickness;

        public List<GameObject> Walls;

        static double getRatio(int n)
        {
            return (n == 1 ? 1 : Math.Sqrt(n));
        }

        private void verifyFaces(ref bool[] faces, float ratio)
        {
            walls = new List<ListableClass>[6] { BackSide, FrontSide, LeftSide, RightSide, TopSide, BottomSide };

            for (int k = 0; k < 6; ++k)
            {
                //inspect column
                for (int i = 0; i < ratio; ++i)
                {
                    int num = 0;
                    for (int j = 0; j + i < ratio; ++j)
                    {
                        if (ArrayOfSides[k][j, i])
                            ++num;
                    }
                    if (num == NumberOfPieces)
                    {
                        faces[k] = false;
                    }
                }
                //inspect row
                for (int i = 0; i < ratio; ++i)
                {
                    int nb = 0;
                    for (int j = 0; j < ratio; ++j)
                    {
                        if (ArrayOfSides[k][j, i])
                            ++nb;
                    }
                    if (nb == NumberOfPieces)
                    {
                        faces[k] = false;
                    }
                }
            }
        }

        private Vector3 SetGlobalScale(Transform t, Vector3 g, Vector3 localScale)
        {
            Vector3 local = Vector3.one;
            local.x = g.x / t.lossyScale.x;
            local.y = g.y / t.lossyScale.y;
            local.z = g.z / t.lossyScale.z;
            return local;
        }

        private void CreateWalls(int _NumberOfPieces)
        {
            walls = new List<ListableClass>[6] { BackSide, FrontSide, LeftSide, RightSide, TopSide, BottomSide };

            if (NumberOfPieces <= 0)
                return;
            if (NumberOfPieces != 1)
                verifyFaces(ref good_faces, NumberOfPieces);
            for (int nb = 0; nb < 6; ++nb)
                if (good_faces[nb] == false)
                    Debug.Log("Changement done to Face: " + walls_name[nb] + " does not repect the rule: One row/column cannot be empty");


            //order : back, forward, right, left, up down
            Vector3[] _positions = { new Vector3 { x = 0, y = _size.y / 2, z = _size.z / 2f },
                         new Vector3 { x = 0, y = _size.y / 2, z = -_size.z / 2f },
                         new Vector3 { x = -_size.x / 2f, y = _size.y / 2, z = 0 },
                         new Vector3 { x = _size.x / 2f, y = _size.y / 2, z = 0 },
                         new Vector3 { x = 0, y = _size.y, z = 0 },
                         new Vector3 { x = 0, y = 0, z = 0 }
        };

            //order : back, forward, right, left, up, down
            Vector3[] _positions_zero = { new Vector3 { x = _size.x / 2, y = _size.y / 2, z = _size.z / 2 },
                         new Vector3 { x = -_size.x / 2, y = _size.y / 2, z = -_size.z / 2 },
                         new Vector3 { x = -_size.x / 2f, y = _size.y / 2, z = -_size.x / 2f },
                         new Vector3 { x = _size.x / 2f, y = _size.y / 2, z = _size.x / 2f },
                         new Vector3 { x = 0, y = _size.y, z = _size.x / 2f },
                         new Vector3 { x = 0, y = 0, z = _size.x / 2f }
        };

            Tuple<Vector3, Vector3>[] _rotations = { new Tuple<Vector3, Vector3>(Vector3.back, transform.up),
                        new Tuple<Vector3, Vector3>(Vector3.forward, transform.up),
                        new Tuple<Vector3, Vector3>(Vector3.right, transform.up),
                        new Tuple<Vector3, Vector3>(Vector3.left, transform.up),
                        new Tuple<Vector3, Vector3>(Vector3.down, transform.forward),
                        new Tuple<Vector3, Vector3>(Vector3.up, -transform.forward)
        };

            //Init wall with pos and rotation
            int k = 0;
            for (int i = 0; i < (_NumberOfPieces * NUMBER_OF_FACES); ++i)
            {
                if (i != 0 && i % _NumberOfPieces == 0)
                    ++k;
                Walls.Add(Instantiate(myPrefab, _positions[k], Quaternion.LookRotation(_rotations[k].Item1, _rotations[k].Item2), transform));
            }

            // Set Wall size
            int num = 0;
            foreach (GameObject wall in Walls)
            {
                if (num >= (NUMBER_OF_FACES * _NumberOfPieces) - (_NumberOfPieces * 2))
                    wall.transform.localScale = new Vector3((_size.x + WallThickness) / NumberOfPieces, (_size.x + WallThickness) / NumberOfPieces, WallThickness);
                else if (num >= (NUMBER_OF_FACES * _NumberOfPieces) - (_NumberOfPieces * 4))
                    wall.transform.localScale = new Vector3((_size.x + WallThickness) / NumberOfPieces, (_size.y + WallThickness) / NumberOfPieces, WallThickness);
                else
                    wall.transform.localScale = new Vector3((_size.z + WallThickness) / NumberOfPieces, (_size.y + WallThickness) / NumberOfPieces, WallThickness);
                ++num;
            }

            // Adjust pos
            k = 0;
            if (_NumberOfPieces > 1)
            {
                //Because we previously modified their size, we need to recalculate their position
                _positions_zero[0].x = Walls[0].transform.position.x + (_size.x / 2) - (Walls[0].transform.localScale.x / 2) + (WallThickness / 2);
                _positions_zero[0].y = Walls[0].transform.position.y - (_size.y / 2) + (Walls[0].transform.localScale.y / 2) - (WallThickness / 2);
                _positions_zero[1].x = Walls[2 * _NumberOfPieces - 1].transform.position.x - (_size.x / 2) + (Walls[2 * _NumberOfPieces - 1].transform.localScale.x / 2) - (WallThickness / 2);
                _positions_zero[1].y = Walls[2 * _NumberOfPieces - 1].transform.position.y - (_size.y / 2) + (Walls[2 * _NumberOfPieces - 1].transform.localScale.y / 2) - (WallThickness / 2);
                _positions_zero[2].z = Walls[3 * _NumberOfPieces - 1].transform.position.z - (_size.x / 2) + (Walls[3 * _NumberOfPieces - 1].transform.localScale.x / 2) - (WallThickness / 2);
                _positions_zero[2].y = Walls[3 * _NumberOfPieces - 1].transform.position.y - (_size.y / 2) + (Walls[3 * _NumberOfPieces - 1].transform.localScale.y / 2) - (WallThickness / 2);
                _positions_zero[3].z = Walls[4 * _NumberOfPieces - 1].transform.position.z - (_size.x / 2) + (Walls[4 * _NumberOfPieces - 1].transform.localScale.x / 2) - (WallThickness / 2);
                _positions_zero[3].y = Walls[4 * _NumberOfPieces - 1].transform.position.y - (_size.y / 2) + (Walls[4 * _NumberOfPieces - 1].transform.localScale.y / 2) - (WallThickness / 2);
                _positions_zero[4].x = Walls[5 * _NumberOfPieces - 1].transform.position.x - (_size.x / 2) + (Walls[5 * _NumberOfPieces - 1].transform.localScale.x / 2) - (WallThickness / 2);
                _positions_zero[4].z = Walls[5 * _NumberOfPieces - 1].transform.position.z + (_size.x / 2) - (Walls[5 * _NumberOfPieces - 1].transform.localScale.x / 2) + (WallThickness / 2);
                _positions_zero[5].z = _positions_zero[4].z;
                _positions_zero[5].x = Walls[6 * _NumberOfPieces - 1].transform.position.x - (_size.x / 2) + (Walls[6 * _NumberOfPieces - 1].transform.localScale.x / 2) - (WallThickness / 2);
                for (int i = 0; i < NUMBER_OF_FACES * _NumberOfPieces; i += _NumberOfPieces)
                {
                    num = 0;
                    for (int w = 0; w < NumberOfPieces; ++w)
                    {
                        for (int j = 0; j < NumberOfPieces; ++j)
                        {
                            if (i < (_NumberOfPieces))
                                Walls[i + num].transform.position = new Vector3(_positions_zero[k].x - (j * Walls[i + num].transform.localScale.x), _positions_zero[k].y + (w * Walls[i].transform.localScale.y), _positions_zero[k].z);
                            else if (i < (2 * _NumberOfPieces))
                                Walls[i + num].transform.position = new Vector3(_positions_zero[k].x + (j * Walls[i + num].transform.localScale.x), _positions_zero[k].y + (w * Walls[i].transform.localScale.y), _positions_zero[k].z);
                            else if (i < (3 * _NumberOfPieces))
                                Walls[i + num].transform.position = new Vector3(_positions_zero[k].x, _positions_zero[k].y + (w * Walls[i].transform.localScale.y), _positions_zero[k].z + (j * Walls[i + num].transform.localScale.x));
                            else if (i < (4 * _NumberOfPieces))
                                Walls[i + num].transform.position = new Vector3(_positions_zero[k].x, _positions_zero[k].y + (w * Walls[i].transform.localScale.y), _positions_zero[k].z + (j * Walls[i + num].transform.localScale.x));
                            else
                                Walls[i + num].transform.position = new Vector3(_positions_zero[k].x + (w * Walls[i].transform.localScale.x), _positions_zero[k].y, _positions_zero[k].z - (j * Walls[i + num].transform.localScale.x));
                            ++num;
                        }
                    }
                    ++k;
                }
            }

            walls = new List<ListableClass>[6] { BackSide, FrontSide, LeftSide, RightSide, TopSide, BottomSide };
        }

        private void ChamberStart()
        {
            good_faces = new bool[6] { true, true, true, true, true, true };
            Walls = new List<GameObject>();

            StaticNumberOfPieces = NumberOfPieces;
            int _NumberOfPieces = NumberOfPieces * NumberOfPieces;

            CreateWalls(_NumberOfPieces);

            //order : back, forward, right, left, up, down
            // Set Wall material
            ChangeWallsMaterial(_NumberOfPieces, ref good_faces, ref walls);
            StaticWallMaterial = WallMaterial;
        }

        // Start is called before the first frame update
        protected new void Start()
        {
            //Call Area Start function
            base.Start();
            Screen.lockCursor = true;
            NumberOfPieces = 1;
            changeY(NumberOfPieces);
            ChamberStart();
            transform.position = pos;
        }

        private void ChangeWallsMaterial(int _NumberOfPieces, ref bool[] good_faces, ref List<ListableClass>[] walls)
        {
            int k = 0;

            if (conf == 1)
            {
                ArrayOfSides[0][0, 0] = false;
                ArrayOfSides[1][0, 0] = false;
                ArrayOfSides[2][0, 0] = false;
                ArrayOfSides[3][0, 0] = false;
                ArrayOfSides[4][0, 0] = false;
                ArrayOfSides[5][0, 0] = false;
            }
            else if (conf == 2)
            {
                ArrayOfSides[0][0, 0] = false;
                ArrayOfSides[1][0, 0] = false;
                ArrayOfSides[2][0, 0] = true;
                ArrayOfSides[3][0, 0] = true;
                ArrayOfSides[4][0, 0] = false;
                ArrayOfSides[5][0, 0] = false;
            }
            else if (conf == 3)
            {
                ArrayOfSides[0][0, 0] = true;
                ArrayOfSides[1][0, 0] = false;
                ArrayOfSides[2][0, 0] = true;
                ArrayOfSides[3][0, 0] = true;
                ArrayOfSides[4][0, 0] = true;
                ArrayOfSides[5][0, 0] = false;
            }
            else if (conf == 4) {
                ArrayOfSides[0][0, 0] = true;
                ArrayOfSides[1][0, 0] = true;
                ArrayOfSides[2][0, 0] = true;
                ArrayOfSides[3][0, 0] = true;
                ArrayOfSides[4][0, 0] = true;
                ArrayOfSides[5][0, 0] = true;
            }

            Debug.Log("Display with conf: " + conf + " " + _NumberOfPieces);

            for (int i = 0; i < 6; ++i)
            {
                /* In case the rule has been broken display whole face */
                /*if (good_faces[i] == false)
                    for (int j = _NumberOfPieces * i; j < _NumberOfPieces * (i + 1); ++j)
                        Walls[j].GetComponent<Renderer>().material = WallMaterial;
                else
                {
                    for (int j = 0; j < NumberOfPieces; ++j)
                    {
                        for (k = 0; k < NumberOfPieces; ++k)
                        {
                            Debug.Log("Je suis la réponse : " + ArrayOfSides[i][k, j]);
                            Debug.Log(i + " " + k + " " + j);
                            if (!ArrayOfSides[i][k, j])
                            {
                                Walls[(((NumberOfPieces - 1) - k) * NumberOfPieces) + (j) + (i * _NumberOfPieces)].GetComponent<MeshRenderer>().enabled = true;
                                Walls[(((NumberOfPieces - 1) - k) * NumberOfPieces) + (j) + (i * _NumberOfPieces)].GetComponent<BoxCollider>().enabled = true;
                                Walls[(((NumberOfPieces - 1) - k) * NumberOfPieces) + (j) + (i * _NumberOfPieces)].GetComponent<Renderer>().material = WallMaterial;
                            }
                            else
                            {
                                Walls[(((NumberOfPieces - 1) - k) * NumberOfPieces) + (j) + (i * _NumberOfPieces)].GetComponent<MeshRenderer>().enabled = false;
                                Walls[(((NumberOfPieces - 1) - k) * NumberOfPieces) + (j) + (i * _NumberOfPieces)].GetComponent<BoxCollider>().enabled = false;
                            }
                        }
                    }
                }

            }
        }

        protected new void Update()
        {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                conf = 1;
                print("0");
            } else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                conf = 2;
                print("1");
            } else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                conf = 3;
                print("2");
            } else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                conf = 4;
                print("3");
            }

            /* Chamber code */
            /*if (StaticWallMaterial != WallMaterial)
            {
                ChangeWallsMaterial(NumberOfPieces * NumberOfPieces, ref good_faces, ref walls);
                StaticWallMaterial = WallMaterial;
            }
            //When we dynamickly update the number of faces, the size of each side aren't updated at the same time
            //We need to make sure that case never happen by first making sure array's size is the same length as NumberOfPieces' value

            /* Area code */
            /*EEAreaUpdate(_area);
            ChamberUpdatePlanes();

            EECamera eecam = Array.Find(UnityEngine.Camera.main.GetComponents<EECamera>(), camera => camera.area == _area);
            if (_collider.bounds.Contains(UnityEngine.Camera.main.transform.position))
            {
                if (!eecam)
                {
                    eecam = UnityEngine.Camera.main.gameObject.AddComponent<EECamera>();
                    eecam.area = _area;
                }
            }
            else if (eecam)
            {
                Destroy(eecam);
            }
        }

        private void FixedUpdate()
        {
            if (conf != static_conf)
            {
                static_conf = conf;
                ChangeWallsMaterial(1, ref good_faces, ref walls);
            }
        }


        private void ChamberUpdatePlanes()
        {
            _camera.transform.position = Camera.main.transform.position;
            _camera.transform.rotation = Camera.main.transform.rotation;
            EEAreaSetCameraPosition(_area, _camera.transform.position);

            //set plane size
            _planeBack.size = new Vector2(_size.x, _size.y);
            _planeFront.size = new Vector2(_size.x, _size.y);
            _planeRight.size = new Vector2(_size.z, _size.y);
            _planeLeft.size = new Vector2(_size.z, _size.y);
            _planeTop.size = new Vector2(_size.x, _size.z);
            _planeBottom.size = new Vector2(_size.x, _size.z);

            //update each plan with new camera matrix
            Matrix4x4 transformMatrix;
            List<KeyValuePair<KeyValuePair<AreaPlane, Vector3>, float>> distances = new List<KeyValuePair<KeyValuePair<AreaPlane, Vector3>, float>>();

            for (short i = 0; i < 6; ++i)
            {
                distances.Add(new KeyValuePair<KeyValuePair<AreaPlane, Vector3>, float>(_sortAreaPlane[i],
                    Mathf.Sqrt(Mathf.Pow(_sortAreaPlane[i].Key.transform.position.x - _camera.transform.position.x, (float)2.0) +
                    Mathf.Pow(_sortAreaPlane[i].Key.transform.position.y - _camera.transform.position.y, (float)2.0) +
                    Mathf.Pow(_sortAreaPlane[i].Key.transform.position.z - _camera.transform.position.z, (float)2.0))));
                _sortAreaPlane[i].Key.gameObject.layer |= LayerMask.NameToLayer("toto");
            }

            //sort distances
            distances.Sort(delegate (KeyValuePair<KeyValuePair<AreaPlane, Vector3>, float> a, KeyValuePair<KeyValuePair<AreaPlane, Vector3>, float> b)
            {
                return a.Value.CompareTo(b.Value);
            });

            //Reverse array so it is descending
            distances.Reverse();

            foreach (var item in distances)
            {
                EEAreaGetTransformMatrix(_area, new Vector3(1, 1, 1), out transformMatrix);
                item.Key.Key.UpdatePlane(Camera.main.worldToCameraMatrix, transformMatrix);
                item.Key.Key.gameObject.layer ^= LayerMask.NameToLayer("toto");
            }
        }

        public bool check()
        {
            good_faces = new bool[6] { true, true, true, true, true, true };
            verifyFaces(ref good_faces, NumberOfPieces);
            for (int nb = 0; nb < 6; ++nb)
                if (good_faces[nb] == false)
                {
                    return false;
                }
            return true;
        }

        public void UpdateFaces()
        {
            // That means we not in run time
            if (Walls.Count == 0)
            {
                return;
            }

            if (PlayerPrefs.GetInt("Length") > 1)
                check();

            //If the length has not been changed we just update walls visibility
            if (StaticNumberOfPieces == PlayerPrefs.GetInt("Length"))
            {
                ChangeWallsMaterial(NumberOfPieces * NumberOfPieces, ref good_faces, ref walls);
                StaticNumberOfPieces = NumberOfPieces;
                return;
            }

            //If the length has been changed we have to destroy all walls to rebuilt them at the correct settings
            foreach (GameObject to_destroy in Walls)
            {
                Destroy(to_destroy);
            }
            Walls.Clear();


            //recreate wall
            CreateWalls(NumberOfPieces * NumberOfPieces);
            //reset material
            //ChangeWallsMaterial(NumberOfPieces * NumberOfPieces, ref good_faces, ref walls);
        }
    }
}*/



namespace EuclidEngine
{
    [AddComponentMenu("Euclid Engine/Non Euclidian Room")]
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody), typeof(Material))]


    [System.Serializable]
    public class NonVisualEuclidianRoom3 : Area2
    {
        /* Beautiful print for editor */
        public static int Y = 0;
        // 6  because that is the number of sides
        [SerializeField] public bool[][,] ArrayOfSides = new bool[6][,] { new bool[0, 0], new bool[0, 0], new bool[0, 0], new bool[0, 0], new bool[0, 0], new bool[0, 0] };
        public void changeY(int new_y)
        {
            Y = new_y;
            //boolArray2D = new bool[new_y, new_y];

            for (int i = 0; i < 6; ++i)
            {
                ArrayOfSides[i] = new bool[new_y, new_y];
            }

        }
        /* End of beautiful Editor */

        /* Const var*/
        private const int NUMBER_OF_FACES = 6;
        private bool first_time = true;
        private GameObject objToSpawn;
        public Vector3 pos;
        private int conf = 1;
        private int static_conf = 1;
        string[] walls_name = { "BackSide", "FrontSide", "LeftSide", "RightSide", "TopSide", "BottomSide" };

        public void ChangeArraySize(ref bool[,] array, int size)
        {
            array = new bool[size, size];
        }

        /* Check Array */
        public List<ListableClass> BackSide = new List<ListableClass>();
        public List<ListableClass> FrontSide = new List<ListableClass>();
        public List<ListableClass> LeftSide = new List<ListableClass>();
        public List<ListableClass> RightSide = new List<ListableClass>();
        public List<ListableClass> TopSide = new List<ListableClass>();
        public List<ListableClass> BottomSide = new List<ListableClass>();

        private List<ListableClass>[] walls;
        private bool[] good_faces;

        /* Mutable var */
        private int StaticNumberOfPieces;
        [SerializeField] private int NumberOfPieces = 1;
        [SerializeField] private int EditorNumberOfPieces = 1;
        public GameObject myPrefab;

        public Material WallMaterial;
        private Material StaticWallMaterial = null;
        public float WallThickness;

        public List<GameObject> Walls;

        static double getRatio(int n)
        {
            return (n == 1 ? 1 : Math.Sqrt(n));
        }
        public int getConfig()
        {
            return conf;
        }
        private void load_array_first_time()
        {
            string[] options = new string[]
            {
                "Back Side", "Front Side", "Right Side", "Left Side", "Top Side", "Bottom Side"
            };

            if (!PlayerPrefs.HasKey("Length"))
            {
                return;
            }

            for (int i = 0; i < 6; ++i)
            {
                if (PlayerPrefs.HasKey(options[i]))
                {
                    for (int j = 0; j < NumberOfPieces; ++j)
                    {
                        string[] side = PlayerPrefs.GetString(options[i]).Split(char.Parse("\n"));
                        int[] nums = new int[NumberOfPieces];
                        for (int k = 0; k < NumberOfPieces; ++k)
                        {
                            Debug.Log(side[k]);
                            nums = Array.ConvertAll<string, int>(side[k].Split(','), int.Parse);
                            for (int l = 0; l < NumberOfPieces; ++l)
                            {
                                ArrayOfSides[i][k, l] = (nums[l] == 1) ? true : false;
                            }
                        }
                    }
                }
            }
        }

        private void verifyFaces(ref bool[] faces, float ratio)
        {
            walls = new List<ListableClass>[6] { BackSide, FrontSide, LeftSide, RightSide, TopSide, BottomSide };

            for (int k = 0; k < 6; ++k)
            {
                //inspect column
                for (int i = 0; i < ratio; ++i)
                {
                    int num = 0;
                    for (int j = 0; j + i < ratio; ++j)
                    {
                        if (ArrayOfSides[k][j, i])
                            ++num;
                    }
                    if (num == NumberOfPieces)
                    {
                        faces[k] = false;
                    }
                }
                //inspect row
                for (int i = 0; i < ratio; ++i)
                {
                    int nb = 0;
                    for (int j = 0; j < ratio; ++j)
                    {
                        if (ArrayOfSides[k][j, i])
                            ++nb;
                    }
                    if (nb == NumberOfPieces)
                    {
                        faces[k] = false;
                    }
                }
            }
        }

        private Vector3 SetGlobalScale(Transform t, Vector3 g, Vector3 localScale)
        {
            Vector3 local = Vector3.one;
            local.x = g.x / t.lossyScale.x;
            local.y = g.y / t.lossyScale.y;
            local.z = g.z / t.lossyScale.z;
            return local;
        }

        private void CreateWalls(int _NumberOfPieces)
        {
            walls = new List<ListableClass>[6] { BackSide, FrontSide, LeftSide, RightSide, TopSide, BottomSide };

            if (NumberOfPieces <= 0)
                return;
            if (NumberOfPieces != 1)
                verifyFaces(ref good_faces, NumberOfPieces);
            for (int nb = 0; nb < 6; ++nb)
                if (good_faces[nb] == false)
                    Debug.Log("Changement done to Face: " + walls_name[nb] + " does not repect the rule: One row/column cannot be empty");


            //order : back, forward, right, left, up down
            Vector3[] _positions = { new Vector3 { x = 0, y = _size.y / 2, z = _size.z / 2f },
                         new Vector3 { x = 0, y = _size.y / 2, z = -_size.z / 2f },
                         new Vector3 { x = -_size.x / 2f, y = _size.y / 2, z = 0 },
                         new Vector3 { x = _size.x / 2f, y = _size.y / 2, z = 0 },
                         new Vector3 { x = 0, y = _size.y, z = 0 },
                         new Vector3 { x = 0, y = 0, z = 0 }
        };

            //order : back, forward, right, left, up, down
            Vector3[] _positions_zero = { new Vector3 { x = _size.x / 2, y = _size.y / 2, z = _size.z / 2 },
                         new Vector3 { x = -_size.x / 2, y = _size.y / 2, z = -_size.z / 2 },
                         new Vector3 { x = -_size.x / 2f, y = _size.y / 2, z = -_size.x / 2f },
                         new Vector3 { x = _size.x / 2f, y = _size.y / 2, z = _size.x / 2f },
                         new Vector3 { x = 0, y = _size.y, z = _size.x / 2f },
                         new Vector3 { x = 0, y = 0, z = _size.x / 2f }
        };

            Tuple<Vector3, Vector3>[] _rotations = { new Tuple<Vector3, Vector3>(Vector3.back, transform.up),
                        new Tuple<Vector3, Vector3>(Vector3.forward, transform.up),
                        new Tuple<Vector3, Vector3>(Vector3.right, transform.up),
                        new Tuple<Vector3, Vector3>(Vector3.left, transform.up),
                        new Tuple<Vector3, Vector3>(Vector3.down, transform.forward),
                        new Tuple<Vector3, Vector3>(Vector3.up, -transform.forward)
        };

            //Init wall with pos and rotation
            int k = 0;
            for (int i = 0; i < (_NumberOfPieces * NUMBER_OF_FACES); ++i)
            {
                if (i != 0 && i % _NumberOfPieces == 0)
                    ++k;
                Walls.Add((Instantiate(myPrefab, _positions[k], Quaternion.LookRotation(_rotations[k].Item1, _rotations[k].Item2))));
                Walls[i].transform.parent = objToSpawn.transform;
            }

            // Set Wall size
            int num = 0;
            foreach (GameObject wall in Walls)
            {
                if (num >= (NUMBER_OF_FACES * _NumberOfPieces) - (_NumberOfPieces * 2))
                    wall.transform.localScale = new Vector3((_size.x + WallThickness) / NumberOfPieces, (_size.x + WallThickness) / NumberOfPieces, WallThickness);
                else if (num >= (NUMBER_OF_FACES * _NumberOfPieces) - (_NumberOfPieces * 4))
                    wall.transform.localScale = new Vector3((_size.x + WallThickness) / NumberOfPieces, (_size.y + WallThickness) / NumberOfPieces, WallThickness);
                else
                    wall.transform.localScale = new Vector3((_size.z + WallThickness) / NumberOfPieces, (_size.y + WallThickness) / NumberOfPieces, WallThickness);
                ++num;
            }

            // Adjust pos
            k = 0;
            if (_NumberOfPieces > 1)
            {
                //Because we previously modified their size, we need to recalculate their position
                _positions_zero[0].x = Walls[0].transform.position.x + (_size.x / 2) - (Walls[0].transform.localScale.x / 2) + (WallThickness / 2);
                _positions_zero[0].y = Walls[0].transform.position.y - (_size.y / 2) + (Walls[0].transform.localScale.y / 2) - (WallThickness / 2);
                _positions_zero[1].x = Walls[2 * _NumberOfPieces - 1].transform.position.x - (_size.x / 2) + (Walls[2 * _NumberOfPieces - 1].transform.localScale.x / 2) - (WallThickness / 2);
                _positions_zero[1].y = Walls[2 * _NumberOfPieces - 1].transform.position.y - (_size.y / 2) + (Walls[2 * _NumberOfPieces - 1].transform.localScale.y / 2) - (WallThickness / 2);
                _positions_zero[2].z = Walls[3 * _NumberOfPieces - 1].transform.position.z - (_size.x / 2) + (Walls[3 * _NumberOfPieces - 1].transform.localScale.x / 2) - (WallThickness / 2);
                _positions_zero[2].y = Walls[3 * _NumberOfPieces - 1].transform.position.y - (_size.y / 2) + (Walls[3 * _NumberOfPieces - 1].transform.localScale.y / 2) - (WallThickness / 2);
                _positions_zero[3].z = Walls[4 * _NumberOfPieces - 1].transform.position.z - (_size.x / 2) + (Walls[4 * _NumberOfPieces - 1].transform.localScale.x / 2) - (WallThickness / 2);
                _positions_zero[3].y = Walls[4 * _NumberOfPieces - 1].transform.position.y - (_size.y / 2) + (Walls[4 * _NumberOfPieces - 1].transform.localScale.y / 2) - (WallThickness / 2);
                _positions_zero[4].x = Walls[5 * _NumberOfPieces - 1].transform.position.x - (_size.x / 2) + (Walls[5 * _NumberOfPieces - 1].transform.localScale.x / 2) - (WallThickness / 2);
                _positions_zero[4].z = Walls[5 * _NumberOfPieces - 1].transform.position.z + (_size.x / 2) - (Walls[5 * _NumberOfPieces - 1].transform.localScale.x / 2) + (WallThickness / 2);
                _positions_zero[5].z = _positions_zero[4].z;
                _positions_zero[5].x = Walls[6 * _NumberOfPieces - 1].transform.position.x - (_size.x / 2) + (Walls[6 * _NumberOfPieces - 1].transform.localScale.x / 2) - (WallThickness / 2);
                for (int i = 0; i < NUMBER_OF_FACES * _NumberOfPieces; i += _NumberOfPieces)
                {
                    num = 0;
                    for (int w = 0; w < NumberOfPieces; ++w)
                    {
                        for (int j = 0; j < NumberOfPieces; ++j)
                        {
                            if (i < (_NumberOfPieces))
                                Walls[i + num].transform.position = new Vector3(_positions_zero[k].x - (j * Walls[i + num].transform.localScale.x), _positions_zero[k].y + (w * Walls[i].transform.localScale.y), _positions_zero[k].z);
                            else if (i < (2 * _NumberOfPieces))
                                Walls[i + num].transform.position = new Vector3(_positions_zero[k].x + (j * Walls[i + num].transform.localScale.x), _positions_zero[k].y + (w * Walls[i].transform.localScale.y), _positions_zero[k].z);
                            else if (i < (3 * _NumberOfPieces))
                                Walls[i + num].transform.position = new Vector3(_positions_zero[k].x, _positions_zero[k].y + (w * Walls[i].transform.localScale.y), _positions_zero[k].z + (j * Walls[i + num].transform.localScale.x));
                            else if (i < (4 * _NumberOfPieces))
                                Walls[i + num].transform.position = new Vector3(_positions_zero[k].x, _positions_zero[k].y + (w * Walls[i].transform.localScale.y), _positions_zero[k].z + (j * Walls[i + num].transform.localScale.x));
                            else
                                Walls[i + num].transform.position = new Vector3(_positions_zero[k].x + (w * Walls[i].transform.localScale.x), _positions_zero[k].y, _positions_zero[k].z - (j * Walls[i + num].transform.localScale.x));
                            ++num;
                        }
                    }
                    ++k;
                }
            }

            walls = new List<ListableClass>[6] { BackSide, FrontSide, LeftSide, RightSide, TopSide, BottomSide };
        }

        private void ChamberStart()
        {
            good_faces = new bool[6] { true, true, true, true, true, true };
            Walls = new List<GameObject>();

            StaticNumberOfPieces = NumberOfPieces;
            int _NumberOfPieces = NumberOfPieces * NumberOfPieces;

            CreateWalls(_NumberOfPieces);

            //order : back, forward, right, left, up, down
            // Set Wall material
            ChangeWallsMaterial(_NumberOfPieces, ref good_faces, ref walls);
            StaticWallMaterial = WallMaterial;
        }

        // Start is called before the first frame update
        protected new void Start()
        {
            //Call Area Start function
            base.Start();
            objToSpawn = new GameObject("Parent");
            objToSpawn.transform.parent = transform;
            NumberOfPieces = 1;
            changeY(NumberOfPieces);
            ChamberStart();
            transform.position = pos;
        }

        private void ChangeWallsMaterial(int _NumberOfPieces, ref bool[] good_faces, ref List<ListableClass>[] walls)
        {
            int k = 0;

            if (conf == 1)
            {
                ArrayOfSides[0][0, 0] = false;
                ArrayOfSides[1][0, 0] = false;
                ArrayOfSides[2][0, 0] = false;
                ArrayOfSides[3][0, 0] = false;
                ArrayOfSides[4][0, 0] = false;
                ArrayOfSides[5][0, 0] = false;
            }
            else if (conf == 2)
            {
                ArrayOfSides[0][0, 0] = false;
                ArrayOfSides[1][0, 0] = false;
                ArrayOfSides[2][0, 0] = true;
                ArrayOfSides[3][0, 0] = true;
                ArrayOfSides[4][0, 0] = false;
                ArrayOfSides[5][0, 0] = false;
            }
            else if (conf == 3)
            {
                ArrayOfSides[0][0, 0] = true;
                ArrayOfSides[1][0, 0] = false;
                ArrayOfSides[2][0, 0] = true;
                ArrayOfSides[3][0, 0] = true;
                ArrayOfSides[4][0, 0] = true;
                ArrayOfSides[5][0, 0] = false;
            }
            else if (conf == 4)
            {
                ArrayOfSides[0][0, 0] = true;
                ArrayOfSides[1][0, 0] = true;
                ArrayOfSides[2][0, 0] = true;
                ArrayOfSides[3][0, 0] = true;
                ArrayOfSides[4][0, 0] = true;
                ArrayOfSides[5][0, 0] = true;
            }

            for (int i = 0; i < 6; ++i)
            {
                /* In case the rule has been broken display whole face */
                if (good_faces[i] == false)
                    for (int j = _NumberOfPieces * i; j < _NumberOfPieces * (i + 1); ++j)
                        Walls[j].GetComponent<Renderer>().material = WallMaterial;
                else
                {
                    for (int j = 0; j < NumberOfPieces; ++j)
                    {
                        for (k = 0; k < NumberOfPieces; ++k)
                        {
                            if (!ArrayOfSides[i][k, j])
                            {
                                Walls[(((NumberOfPieces - 1) - k) * NumberOfPieces) + (j) + (i * _NumberOfPieces)].GetComponent<MeshRenderer>().enabled = true;
                                Walls[(((NumberOfPieces - 1) - k) * NumberOfPieces) + (j) + (i * _NumberOfPieces)].GetComponent<BoxCollider>().enabled = true;
                                Walls[(((NumberOfPieces - 1) - k) * NumberOfPieces) + (j) + (i * _NumberOfPieces)].GetComponent<Renderer>().material = WallMaterial;
                            }
                            else
                            {
                                Walls[(((NumberOfPieces - 1) - k) * NumberOfPieces) + (j) + (i * _NumberOfPieces)].GetComponent<MeshRenderer>().enabled = false;
                                Walls[(((NumberOfPieces - 1) - k) * NumberOfPieces) + (j) + (i * _NumberOfPieces)].GetComponent<BoxCollider>().enabled = false;
                            }
                        }
                    }
                }
                first_time = false;
            }
        }

        protected new void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                conf = 1;
                print("0");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                conf = 2;
                print("1");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                conf = 3;
                print("2");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                conf = 4;
                print("3");
            }
            /* Chamber code */
            if (StaticWallMaterial != WallMaterial)
            {
                ChangeWallsMaterial(NumberOfPieces * NumberOfPieces, ref good_faces, ref walls);
                StaticWallMaterial = WallMaterial;
            }
            //When we dynamickly update the number of faces, the size of each side aren't updated at the same time
            //We need to make sure that case never happen by first making sure array's size is the same length as NumberOfPieces' value

            /* Area code */
            EEAreaUpdate(_area);
            ChamberUpdatePlanes();

            EECamera eecam = Array.Find(UnityEngine.Camera.main.GetComponents<EECamera>(), camera => camera.area == _area);
            if (_collider.bounds.Contains(UnityEngine.Camera.main.transform.position))
            {
                if (!eecam)
                {
                    eecam = UnityEngine.Camera.main.gameObject.AddComponent<EECamera>();
                    eecam.area = _area;
                }
            }
            else if (eecam)
            {
                Destroy(eecam);
            }
        }

        private void FixedUpdate()
        {
            if (conf != static_conf)
            {
                static_conf = conf;
                ChangeWallsMaterial(1, ref good_faces, ref walls);
            }
        }

        private void ChamberUpdatePlanes()
        {
            _camera.transform.position = Camera.main.transform.position;
            _camera.transform.rotation = Camera.main.transform.rotation;
            EEAreaSetCameraPosition(_area, _camera.transform.position);

            //set plane size
            _planeBack.size = new Vector2(_size.x, _size.y);
            _planeFront.size = new Vector2(_size.x, _size.y);
            _planeRight.size = new Vector2(_size.z, _size.y);
            _planeLeft.size = new Vector2(_size.z, _size.y);
            _planeTop.size = new Vector2(_size.x, _size.z);
            _planeBottom.size = new Vector2(_size.x, _size.z);

            //update each plan with new camera matrix
            Matrix4x4 transformMatrix;
            List<KeyValuePair<KeyValuePair<AreaPlane, Vector3>, float>> distances = new List<KeyValuePair<KeyValuePair<AreaPlane, Vector3>, float>>();

            for (short i = 0; i < 6; ++i)
            {
                distances.Add(new KeyValuePair<KeyValuePair<AreaPlane, Vector3>, float>(_sortAreaPlane[i],
                    Mathf.Sqrt(Mathf.Pow(_sortAreaPlane[i].Key.transform.position.x - _camera.transform.position.x, (float)2.0) +
                    Mathf.Pow(_sortAreaPlane[i].Key.transform.position.y - _camera.transform.position.y, (float)2.0) +
                    Mathf.Pow(_sortAreaPlane[i].Key.transform.position.z - _camera.transform.position.z, (float)2.0))));
                _sortAreaPlane[i].Key.gameObject.layer |= LayerMask.NameToLayer("toto");
            }

            //sort distances
            distances.Sort(delegate (KeyValuePair<KeyValuePair<AreaPlane, Vector3>, float> a, KeyValuePair<KeyValuePair<AreaPlane, Vector3>, float> b)
            {
                return a.Value.CompareTo(b.Value);
            });

            //Reverse array so it is descending
            distances.Reverse();

            foreach (var item in distances)
            {
                EEAreaGetTransformMatrix(_area, new Vector3(1, 1, 1), out transformMatrix);
                item.Key.Key.UpdatePlane(Camera.main.worldToCameraMatrix, transformMatrix);
                item.Key.Key.gameObject.layer ^= LayerMask.NameToLayer("toto");
            }
        }

        public bool check()
        {
            good_faces = new bool[6] { true, true, true, true, true, true };
            verifyFaces(ref good_faces, NumberOfPieces);
            for (int nb = 0; nb < 6; ++nb)
                if (good_faces[nb] == false)
                {
                    Debug.Log("Changement done to Face: " + walls_name[nb] + " does not repect the rule: One row/column cannot be empty");
                    return false;
                }
            return true;
        }

        public void UpdateFaces()
        {
            // That means we not in run time
            if (Walls.Count == 0)
            {
                Debug.Log("Wall count == 0");
                return;
            }

            if (PlayerPrefs.GetInt("Length") > 1)
                check();

            //If the length has not been changed we just update walls visibility
            if (StaticNumberOfPieces == PlayerPrefs.GetInt("Length"))
            {
                Debug.Log(NumberOfPieces * NumberOfPieces);
                Debug.Log(ArrayOfSides[0][0, 0]);
                ChangeWallsMaterial(NumberOfPieces * NumberOfPieces, ref good_faces, ref walls);
                Debug.Log("Just update visibility");
                StaticNumberOfPieces = NumberOfPieces;
                return;
            }

            //If the length has been changed we have to destroy all walls to rebuilt them at the correct settings
            Debug.Log("Destroy wall");
            foreach (GameObject to_destroy in Walls)
            {
                Destroy(to_destroy);
            }
            Walls.Clear();

            Debug.Log("Wall count: " + Walls.Count);

            //recreate wall
            CreateWalls(NumberOfPieces * NumberOfPieces);
            //reset material
            ChangeWallsMaterial(NumberOfPieces * NumberOfPieces, ref good_faces, ref walls);
        }
    }
}

[System.Serializable]
public class ListableClass
{
    public List<bool> row = new List<bool> { false };
}

public class BigTest : MonoBehaviour
{
    public static int Y;
    [System.Serializable]
    public class Column
    {
        public bool[] rows = new bool[Y];
    }

    public Column[] columns = new Column[Y];
}
