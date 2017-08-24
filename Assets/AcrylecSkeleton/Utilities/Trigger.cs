    public class Trigger
    {
        private bool _value;

        public bool Value
        {
            get
            {
                var tempBoolean = _value;
                if (_value)
                    _value = false;
                return tempBoolean;
            }
            set { _value = value; }
        }
}