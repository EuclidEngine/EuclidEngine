using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.InteropServices;

namespace EuclidEngine
{
    [AddComponentMenu("Euclid Engine/Non Euclidian Chamber")]
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody), typeof(Material))]

    public class ChamberArea : Area2
    {
        /* Const var*/
        private const int NUMBER_OF_FACES = 6;

        /* Check Array */
        public List<ListableClass> BackSide = new List<ListableClass>();
        public List<ListableClass> FrontSide = new List<ListableClass>();
        public List<ListableClass> LeftSide = new List<ListableClass>();
        public List<ListableClass> RightSide = new List<ListableClass>();
        public List<ListableClass> TopSide = new List<ListableClass>();
        public List<ListableClass> BottomSide = new List<ListableClass>();

        /* Mutable var */
        public int NumberOfPieces = 1;
        public GameObject myPrefab;

        public Material WallMaterial;
        public float WallThickness;

        private List<GameObject> Walls;

        static double getRatio(int n)
        {
            return (n == 1 ? 1 : Math.Sqrt(n));
        }

        private void verifyFaces(ref bool[] faces, float ratio)
        {
            List<ListableClass> []walls = new List<ListableClass>[6]{ BackSide, FrontSide, LeftSide, RightSide, TopSide, BottomSide};
            for (int k = 0; k < 6; ++k)
            {
                //inspect column
                for (int i = 0; i < ratio; ++i)
                {
                    int num = 0;
                    for (int j = 0; j + i < ratio; ++j)
                    {
                        if (walls[k][i + j].row[i])
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
                        if (walls[k][i].row[j])
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

        // Start is called before the first frame update
        protected new void Start()
        {
            //Call Area Start function
            base.Start();
            Walls = new List<GameObject>();
            int _NumberOfPieces = NumberOfPieces * NumberOfPieces;
            float ratio = NumberOfPieces;
            string[] walls_name = { "BackSide", "FrontSide", "LeftSide", "RightSide", "TopSide", "BottomSide" };
            bool[] good_faces = new bool[6]{ true, true, true, true, true, true };
            //if (NumberOfPieces != 1)
            //    verifyFaces(ref good_faces, ratio);
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
                Walls.Add(Instantiate(myPrefab, transform.position + _positions[k], Quaternion.LookRotation(_rotations[k].Item1, _rotations[k].Item2), transform));
            }

            // Set Wall size
            int num = 0;
            foreach (GameObject wall in Walls)
            {
                if (num >= (NUMBER_OF_FACES * _NumberOfPieces) - (_NumberOfPieces * 2))
                    wall.transform.localScale = new Vector3((_size.x + WallThickness) / ratio, (_size.x + WallThickness) / ratio, WallThickness);
                else if (num >= (NUMBER_OF_FACES * _NumberOfPieces) - (_NumberOfPieces * 4))
                    wall.transform.localScale = new Vector3((_size.x + WallThickness) / ratio, (_size.y + WallThickness) / ratio, WallThickness);
                else
                    wall.transform.localScale = new Vector3((_size.z + WallThickness) / ratio, (_size.y + WallThickness) / ratio, WallThickness);
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
                    for (int w = 0; w < ratio; ++w)
                    {
                        for (int j = 0; j < ratio; ++j)
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

            //order : back, forward, right, left, up, down
            // Set Wall material
            List<ListableClass>[] walls = new List<ListableClass>[6] { BackSide, FrontSide, LeftSide, RightSide, TopSide, BottomSide };
            k = 0;
            for (int i = 0; i < 6; ++i)
            {
                /* In case the rule has been borken display whole face */
                if (good_faces[i] == false)
                    for (int j = _NumberOfPieces * i; j < _NumberOfPieces * (i + 1); ++j)
                        Walls[j].GetComponent<Renderer>().material = WallMaterial;
                else
                {
                    for (int j = 0; j < NumberOfPieces; ++j)
                    {
                        for (k = 0; k < NumberOfPieces; ++k)
                        {
                            if (!walls[i][j].row[k])
                                Walls[j + (k * NumberOfPieces) + (i * _NumberOfPieces)].GetComponent<Renderer>().material = WallMaterial;
                            else
                                Walls[j + (k * NumberOfPieces) + (i * _NumberOfPieces)].GetComponent<MeshRenderer>().enabled = false;
                        }
                    }
                }

            }
        }

        protected new void Update()
        {
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


        private void ChamberUpdatePlanes()
        {
            _camera.transform.position = UnityEngine.Camera.main.transform.position;
            _camera.transform.rotation = UnityEngine.Camera.main.transform.rotation;
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
                item.Key.Key.UpdatePlane(UnityEngine.Camera.main.worldToCameraMatrix, transformMatrix);
                item.Key.Key.gameObject.layer ^= LayerMask.NameToLayer("toto");
            }
        }

        /*// Update is called once per frame
        void Update()
        {

        }*/
    }

    [System.Serializable]
    public class ListableClass
    {
        public List<bool> row = new List<bool> { false };
    }

};