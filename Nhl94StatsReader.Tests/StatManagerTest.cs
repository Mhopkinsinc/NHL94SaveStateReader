// <copyright file="StatManagerTest.cs">Copyright ©  2015</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nhl94StatsReader;

namespace Nhl94StatsReader.Tests
{
    [TestClass]
    [PexClass(typeof(StatManager))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class StatManagerTest
    {

        [PexMethod]
        public StatManager Constructor(string SaveStatePath)
        {
            StatManager target = new StatManager(SaveStatePath);
            return target;
            // TODO: add assertions to method StatManagerTest.Constructor(String)
        }
    }
}
