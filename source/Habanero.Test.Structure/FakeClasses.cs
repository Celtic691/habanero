#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
using System.Runtime.Serialization;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;

namespace Habanero.Test.Structure
{

    /// <summary>
    /// Fake so that can use simple constructor.
    /// </summary>
    public class FakePropDef : PropDef
    {
        public FakePropDef()
            : base("prop", typeof(int), PropReadWriteRule.ReadWrite, null)
        {
        }
    }
    /// <summary>
    /// Fake so that can use simple constructor.
    /// </summary>
    public class BOPropSpy : BOProp
    {
        public BOPropSpy()
            : base(new FakePropDef())
        {
        }
        public void SetCurrentValue(object newValue)
        {
            _currentValue = newValue;
        }
    }
}