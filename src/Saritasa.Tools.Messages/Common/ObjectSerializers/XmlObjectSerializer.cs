﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

#if NET452
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Saritasa.Tools.Messages.Common.ObjectSerializers
{
    /// <summary>
    /// Serialize object using XmlSerializer.
    /// </summary>
    public class XmlObjectSerializer : IObjectSerializer
    {
        static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
        {
            CheckCharacters = false,
            Indent = false,
            OmitXmlDeclaration = false,
        };

        /// <inheritdoc />
        public object Deserialize(byte[] bytes, Type type)
        {
            var xmlSerializer = new XmlSerializer(type);
            using (var stream = new MemoryStream(bytes))
            {
                return xmlSerializer.Deserialize(stream);
            }
        }

        /// <inheritdoc />
        public byte[] Serialize(object obj)
        {
            var xmlSerializer = new XmlSerializer(obj.GetType());
            MemoryStream stream = null;
            byte[] bytes = null;
            try
            {
                stream = new MemoryStream();
                using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                {
                    xmlSerializer.Serialize(xmlWriter, obj);
                    bytes = stream.ToArray();
                    stream = null;
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
            return bytes;
        }

        /// <inheritdoc />
        public bool IsText => true;
    }
}
#endif
