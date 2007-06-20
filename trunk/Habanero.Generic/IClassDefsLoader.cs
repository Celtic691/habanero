using System.Collections;

namespace Habanero.Generic
{
    /// <summary>
    /// An interface to model classes that load class definitions from
    /// xml data
    /// </summary>
    public interface IClassDefsLoader
    {
        /// <summary>
        /// Loads class definitions from pre-specified xml data
        /// </summary>
        /// <returns>Returns an IList object containing the definitions</returns>
        IList LoadClassDefs();
    }
}