#pragma warning disable 219
#pragma warning disable 67

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace AcrylecSkeleton //Relevant namespace directive
{
    /// <summary>
    /// Class description
    /// </summary>
    public class CodingConvention : MonoBehaviour
    {
        #region Constants
        public const int CONST1 = 1;
        #endregion

        #region Backing fields
        private int _backingField1;
        #endregion

        #region Private fields
        private int _privateField;
        #endregion

        #region Inspector fields
        [Header("Section 1:")]
        [SerializeField]
        private int _int1;
        [SerializeField]
        private int _int2;

        [Header("Section 2:")]
        [SerializeField]
        private int _int3;
        [SerializeField, Tooltip("This is just a test.")]
        private int _int4;
        #endregion

        #region Public fields
        public int Public1;
        #endregion

        #region Auto properties
        public int Property1 { get; set; }
        #endregion

        #region Property with backing field
        public int BackingField1
        {
            get { return _backingField1; }
            set { _backingField1 = value; }
        }
        #endregion

        #region Delegate
        public delegate void Del();
        #endregion

        #region Event
        public event Del Event;
        public event Action Action;
        #endregion

        public void Playground()
        {
            #region LINQ
            List<int> ints = new List<int>
            {
                1,
                2,
                3,
                4,
                5
            };

            IEnumerable<int> linqExample = 
                from @int in ints
                where @int > 2
                select @int;

            IEnumerable<int> linqExample1 = ints
                .OrderByDescending(i => i)
                .Where(i => i > 2)
                .Select(i => i);
            #endregion

            #region Using Var (Optional)
            //Good
            var var1 = "Test";
            var var2 = 1.0f;

            //Bad
            var var3 = TestMethod(1, "Test");
            #endregion

            #region Nice structure
            string longString = "This is going to be" +
                             "A long string so im" +
                             "really trying not to" +
                             "make it a long one" +
                             "because i know i have" +
                             "to end sometime.";

            //Isolate parts of statements with parentheses, increases readability.
            if ((var1 == longString && var3 == 1) || (var1 != longString && var3 == 2))
            {
            }

            //If a line gets too long, break it down in pieces.
            if ((var1 == longString && var3 == 1) || (var1 != longString && var3 == 2) && 
                (var1 == longString && var3 == 1) || (var1 != longString && var3 == 2))
            {
                
            }
            #endregion
        }

        #region Method structure
        /// <summary>
        /// Method description
        /// </summary>
        /// <param name="firstArg">Argument description</param>
        /// <param name="secondArg">Argument description</param>
        /// <returns>Return description</returns>
        public int TestMethod(int firstArg, string secondArg)
        {
            return 0;
        }

        #endregion

        #region Event Methods
        void EventMethods()
        {
            Action += OnAction;
        }

        /// <summary>
        /// Naming should always be 'On' + 'Event name'
        /// </summary>
        private void OnAction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remember to remove listener when gameobject is destroyed to prevent unwanted scenarios.
        /// </summary>
        void OnDestroy()
        {
            Action -= OnAction;
        }
        #endregion

        #region Commenting
        void Commenting()
        {
            //TODO: Describe what to do here
            //TODO: UNFINISHED: Used when unfinished code needs an explanation.
            //TODO: HACK: For spaghetti solutions and stuff.
        }
        #endregion

        #region Unity Assertion
        public void TakeDamage(float dmg)
        {
            var health = 100f;

            health -= dmg;
            Assert.AreEqual(101f - dmg, health, "Something went wrong.");
            
            //https://docs.unity3d.com/ScriptReference/Assertions.Assert.html
        }
        #endregion

        #region Inspector fields for prefabs & objects
        [SerializeField, Tooltip("Prefab only!")]
        private GameObject __object1;

        //Or
        [SerializeField, Header("Prefabs:")]
        private GameObject __object2;
        #endregion

        #region Debug Log with reference
        public void DebugLogTest()
        {
            Debug.LogWarning("Something went wrong!", transform);
        }
        #endregion
    }
}

#pragma warning restore 219
#pragma warning restore 67