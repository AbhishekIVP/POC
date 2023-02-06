using System;
using com.ivp.rad.common;

namespace com.ivp.commom.commondal
{
    [Serializable]
    public class CommonDALException : RException
    {
        #region constructors
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonDALException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CommonDALException(string message) : base(message)
        {
            //do nothing
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonDALException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public CommonDALException(string message, Exception ex) : base(message, ex)
        {
            //do nothing 
        }
        #endregion
    }
}
