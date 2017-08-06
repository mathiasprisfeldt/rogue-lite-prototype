using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;

namespace AcrylecSkeleton.Utilities
{
    public class NavRepeatDelay : MonoBehaviour
    {
        [Header("Settings:")] [SerializeField, Tooltip("Global amount of time in seconds it takes to start repeating.")]
        private float _globalDelayUntilRepeat;

        [SerializeField, Tooltip("Global time it takes in second for each repeat interval.")]
        private float _globalRepeatInterval;

        [SerializeField] private List<NavRepeatDelayEntry> _entries;

        public List<NavRepeatDelayEntry> DynamicEntries { get; set; }

        public float GlobalRepeatInterval
        {
            get { return _globalRepeatInterval; }
        }

        public float GlobalDelayUntilRepeat
        {
            get { return _globalDelayUntilRepeat; }
        }

        void Start()
        {
            DynamicEntries = new List<NavRepeatDelayEntry>();
            _entries.ForEach(entry => entry.NRDObject = this);
        }

        void Update()
        {
            _entries.ForEach(entry => entry.Update());
            DynamicEntries.ForEach(entry => entry.Update());
        }

        public Vector2 GetDir(string name, InputDevice device = null)
        {
            if (device != null)
            {
                var foundEntry = DynamicEntries.FirstOrDefault(entry => entry.Device == device);
                if (foundEntry != null)
                    return foundEntry.Direction;

                //If the search after already created device is null, create a new entry for the device.
                var repeatDelayEntry = _entries.FirstOrDefault(entry => entry.Name == name);
                if (repeatDelayEntry != null)
                {
                    var newEntry = repeatDelayEntry.Clone();
                    newEntry.Device = device;
                    DynamicEntries.Add(newEntry);
                    return newEntry.Direction;
                }
            }

            var navRepeatDelayEntry = _entries.FirstOrDefault(entry => entry.Name == name);
            return navRepeatDelayEntry != null ? navRepeatDelayEntry.Direction : Vector2.zero;
        }
    }

    /// <summary>
    /// Containes data of each device/kind of input looking for repetetive input delaying.
    /// </summary>
    [Serializable]
    public sealed class NavRepeatDelayEntry
    {
        private float _repeatTimer;
        private float _intervalTimer;

        public string Name;

#pragma warning disable 649
        public string Horizontal;
        public string Vertical;
        public bool IfDeviceIgnoreKey; //If there is a valid device dont use keycode input
        public bool UseDeviceDirection;
        public bool UseGlobalSettings;
        public bool UseActiveDevice;
        public float RepeatAmount; //Time it takes to repeat
        public float IntervalAmount; //Time it takes for each repeat interval

        // ReSharper disable once CollectionNeverUpdated.Global
        public List<NavRepeatInputData> InputDatas;

        private Vector2 _direction;
#pragma warning restore 649

        public Vector2 Direction
        {
            get { return _direction.normalized; }
            private set { _direction = value; }
        }

        public InputDevice Device { get; set; }
        public NavRepeatDelay NRDObject { get; set; }

        public NavRepeatDelayEntry Clone()
        {
            var clone = (NavRepeatDelayEntry) MemberwiseClone();
            clone.InputDatas = InputDatas.ToList();

            return clone;
        }

        public void Update()
        {
            bool shouldIgnoreKey = IfDeviceIgnoreKey && Device != null;

            InputDevice currDevice = Device;
            if (Device == null && UseActiveDevice)
                currDevice = InputManager.ActiveDevice;

            Direction = Vector2.zero;
            var currDir = Vector2.zero;

            //Axis data
            //If we got a device from InControl, take its direction.
            if (currDevice != null && UseDeviceDirection)
                currDir = currDevice.Direction;

            if (Horizontal != string.Empty)
                currDir += Vector2.right * Input.GetAxisRaw(Horizontal);

            if (Vertical != string.Empty)
                currDir += Vector2.up * Input.GetAxisRaw(Vertical);

            //Was pressed
            InputDatas.ForEach(data => currDir += data.GetDirPressed(currDevice, shouldIgnoreKey));
            if (currDir != Vector2.zero)
            {
                Direction = currDir;
                return;
            }

            //Held down
            InputDatas.ForEach(data => currDir += data.GetDirDown(currDevice, shouldIgnoreKey));

            if (currDir != Vector2.zero)
            {
                _intervalTimer += Time.unscaledDeltaTime;
                _repeatTimer += Time.unscaledDeltaTime;

                //If we're using global settings grab them
                var intervalAmount = UseGlobalSettings ? NRDObject.GlobalRepeatInterval : IntervalAmount;
                var repeatAmount = UseGlobalSettings ? NRDObject.GlobalDelayUntilRepeat : RepeatAmount;

                if (_intervalTimer >= intervalAmount && _repeatTimer >= repeatAmount)
                {
                    _intervalTimer = 0;
                    Direction = currDir;
                }
            }
            else
                _repeatTimer = 0;
        }
    }

    /// <summary>
    /// Struct that contains data about what input to check for,
    ///  and what they're equivalent to in direction.
    /// </summary>
    [Serializable]
    public struct NavRepeatInputData
    {
        public KeyCode KeyCode;
        public InputControlType InputControlType;
        public Vector2 Direction;

        /// <summary>
        /// Returns direction from 'held state' input options
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public Vector2 GetDirDown(InputDevice device = null, bool ignoreKey = false)
        {
            if (!ignoreKey && KeyCode != KeyCode.None && Input.GetKey(KeyCode))
                return Direction;

            if (device != null && InputControlType != InputControlType.None && device.GetControl(InputControlType))
                return Direction;

            return Vector2.zero;
        }

        /// <summary>
        /// Returns direction from 'down state' input options
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public Vector2 GetDirPressed(InputDevice device = null, bool ignoreKey = false)
        {
            if (!ignoreKey && KeyCode != KeyCode.None && Input.GetKeyDown(KeyCode))
                return Direction;

            if (device != null && InputControlType != InputControlType.None &&
                device.GetControl(InputControlType).WasPressed)
                return Direction;

            return Vector2.zero;
        }
    }
}