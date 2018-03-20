using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pegas.Rizo
{
    public class GestureRecognizer
    {
        public delegate void Callback(Gesture sender);

        public Callback OnRecognized;

        public abstract class Gesture
        {
            public string ID { get; private set;  }
            public int Priority { get; set; }

            public Callback OnStartRecognizing;
            public Callback OnFailRecognizing;
            public Callback OnRecognized;

            public Gesture(string id)
            {
                ID = id;
            }

            public abstract void Update();
        }

        public class GestureLongHold: Gesture
        {
            public float TressHoldTime { get; set; }

            public GestureLongHold(string id)
                : base(id)
            {
                    
            }

            private bool recognizingStarted =  false;
            private float elapsedTime = 0.0f;

            public override void Update()
            {
                if (Input.GetMouseButtonDown(0) && !recognizingStarted)
                {
                    recognizingStarted = true;
                    elapsedTime = 0.0f;

                    if(OnStartRecognizing != null)
                    {
                        OnStartRecognizing(this);
                    }
                }else if(Input.GetMouseButtonUp(0) && recognizingStarted)
                {
                    recognizingStarted = false;
                    if (elapsedTime >= TressHoldTime)
                    {
                        if(OnRecognized != null)
                        {
                            OnRecognized(this);
                        }                        
                    }else
                    {
                        if(OnFailRecognizing != null)
                        {
                            OnFailRecognizing(this);
                        }
                    }
                }else if(Input.GetMouseButton(0) && recognizingStarted)
                {
                    elapsedTime += Time.deltaTime;
                }
            }
        }

        public class GestureLongMove: Gesture
        {
            public enum Constraint
            {
                None,
                AlongX,
                AlongY                
            };

            public float TressHoldLength { get; set; }
            public Constraint Mode { get; set; }  

            public GestureLongMove(string id)
                : base(id)
            {
                
            }

            private bool recognizingStarted = false;
            private Vector3 startPosition;

            public override void Update()
            {
                if (Input.GetMouseButtonDown(0) && !recognizingStarted)
                {
                    recognizingStarted = true;
                    startPosition = Input.mousePosition;

                    if(OnStartRecognizing != null)
                    {
                        OnStartRecognizing(this);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && recognizingStarted)
                {
                    recognizingStarted = false;
                    var vDelta = Input.mousePosition - startPosition;
                    bool recognized = false;
                    if(Mode == Constraint.AlongX && Math.Abs(vDelta.x) >= TressHoldLength)
                    {
                        Debug.Log("delta X = " + Math.Abs(vDelta.x));
                        recognized = true;

                    } else if (Mode == Constraint.AlongY && Math.Abs(vDelta.y) >= TressHoldLength)
                    {
                        recognized = true;

                    } else if(Mode == Constraint.None && vDelta.magnitude >= TressHoldLength)
                    {
                        recognized = true;
                    }

                    if(recognized)
                    {
                        if(OnRecognized != null)
                        {
                            OnRecognized(this);
                        }
                    }else
                    {
                        if(OnFailRecognizing != null)
                        {
                            OnFailRecognizing(this);
                        }
                    }
                }
            }
        }

        public class GestureSimpleClick : Gesture
        {
            public float MaxTime { get; set; }
            public float MaxRadius { get; set; }

            public GestureSimpleClick(string id)
                :base(id)
            {

            }

            private bool recognizingStarted = false;
            private Vector3 startPosition;
            private float elapsedTime = 0.0f;

            public override void Update()
            {
               if(Input.GetMouseButtonDown(0) && !recognizingStarted)
               {
                    recognizingStarted = true;
                    elapsedTime = 0.0f;
                    startPosition = Input.mousePosition;

                    if(OnStartRecognizing != null)
                    {
                        OnStartRecognizing(this);
                    }

               }else if(recognizingStarted && Input.GetMouseButton(0))
               {
                    elapsedTime += Time.deltaTime;
                    var vDelta = Input.mousePosition - startPosition;
                    if(elapsedTime >= MaxTime || vDelta.magnitude >= MaxRadius)
                    {
                        recognizingStarted = false;
                        if(OnFailRecognizing != null)
                        {
                            OnFailRecognizing(this);
                        }
                    }

               }else if(recognizingStarted && Input.GetMouseButtonUp(0))
               {
                    recognizingStarted = false;

                    elapsedTime += Time.deltaTime;
                    var vDelta = Input.mousePosition - startPosition;
                    if (elapsedTime < MaxTime && vDelta.magnitude < MaxRadius)
                    {
                        if(OnRecognized != null)
                        {
                            OnRecognized(this);
                        }

                    }else
                    {
                        if (OnFailRecognizing != null)
                        {
                            OnFailRecognizing(this);
                        }
                    }
               }
            }
        }

        private List<Gesture> _gestures = new List<Gesture>();
        private List<Gesture> _recognizedGestures = new List<Gesture>();

        public void AddGesture(Gesture gesture)
        {
            _gestures.Add(gesture);

            gesture.OnRecognized = Callback_OnGestureRecognized;
        }

        public void Update()
        {
            _recognizedGestures.Clear();

            foreach (var gesture in _gestures)
            {
                gesture.Update();
            }

            if(_recognizedGestures.Count > 0)
            {
                _recognizedGestures.Sort(Comparsion_Priority);
                
                if(OnRecognized != null)
                {
                    OnRecognized(_recognizedGestures[0]);
                }                
            }
        }

        private void Callback_OnGestureRecognized(Gesture gesture)
        {
            _recognizedGestures.Add(gesture);
        }

        private int Comparsion_Priority(Gesture x, Gesture y)
        {
            return x.Priority - y.Priority;
        }
    }
}
