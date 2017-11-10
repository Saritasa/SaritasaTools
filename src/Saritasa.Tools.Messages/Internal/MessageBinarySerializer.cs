﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Saritasa.Tools.Messages.Abstractions;
using Saritasa.Tools.Messages.Common;

namespace Saritasa.Tools.Messages.Internal
{
    internal class MessageBinarySerializer
    {
        /*
         * Every message in file has following structure:
         * <chunk id> <length> <body>
         * 1 byte, 4 bytes, vary
         */

        private const byte TokenBeginOfCommand = 0x10;
        private const byte TokenId = 0x11;
        private const byte TokenType = 0x12;
        private const byte TokenContentType = 0x13;
        private const byte TokenContent = 0x15;
        private const byte TokenData = 0x16;
        private const byte TokenCreated = 0x17;
        private const byte TokenExecutionDuration = 0x18;
        private const byte TokenStatus = 0x19;
        private const byte TokenErrorDetails = 0x21;
        private const byte TokenErrorMessage = 0x22;
        private const byte TokenErrorType = 0x23;
        private const byte TokenEndOfCommand = 0x50;

        private static readonly Tuple<byte, byte[]> nullChunk = new Tuple<byte, byte[]>(0, null);

        private static readonly byte[] emptyBytes = new byte[] { };

        private readonly IObjectSerializer serializer;

        private readonly Stream stream;

        private readonly object objLock = new object();

        public MessageBinarySerializer(Stream stream, IObjectSerializer serializer)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }
            this.serializer = serializer;
            this.stream = stream;
        }

        private void WriteChunk(byte chunk, byte[] bytes = null)
        {
            stream.WriteByte(chunk);
            if (bytes != null)
            {
                stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        private Tuple<byte, byte[]> ReadChunk()
        {
            var header = new byte[1 + sizeof(int)];
            var n = stream.Read(header, 0, 1);
            if (n == 0)
            {
                return nullChunk;
            }
            if (header[0] == TokenBeginOfCommand || header[0] == TokenEndOfCommand)
            {
                return new Tuple<byte, byte[]>(header[0], null);
            }

            n = stream.Read(header, 1, sizeof(int));
            if (n == 0)
            {
                return nullChunk;
            }
            var length = BitConverter.ToInt32(header, 1);
            var content = new byte[length];

            if (length > 0)
            {
                n = stream.Read(content, 0, content.Length);
                if (n == 0)
                {
                    return nullChunk;
                }
            }
            return new Tuple<byte, byte[]>(header[0], content);
        }

        /// <summary>
        /// Reads the next message from stream.
        /// </summary>
        /// <returns>Message record.</returns>
        public MessageRecord Read()
        {
            var result = new MessageRecord();
            Type errorType = null;
            bool messageStarted = false;
            byte[] content = null;

            lock (objLock)
            {
                Tuple<byte, byte[]> chunk;
                while (!Equals(chunk = ReadChunk(), nullChunk))
                {
                    if (chunk.Item1 == TokenBeginOfCommand)
                    {
                        messageStarted = true;
                    }
                    if (!messageStarted)
                    {
                        continue;
                    }

                    switch (chunk.Item1)
                    {
                        case TokenId:
                            result.Id = new Guid(chunk.Item2);
                            break;
                        case TokenType:
                            result.Type = chunk.Item2[0];
                            break;
                        case TokenContent:
                            content = chunk.Item2;
                            break;
                        case TokenContentType:
                            result.ContentType = Encoding.UTF8.GetString(chunk.Item2);
                            break;
                        case TokenData:
                            result.Data =
                                (IDictionary<string, string>)serializer.Deserialize(chunk.Item2, typeof(IDictionary<string, string>));
                            break;
                        case TokenCreated:
                            result.CreatedAt = DateTime.FromBinary(BitConverter.ToInt64(chunk.Item2, 0));
                            break;
                        case TokenExecutionDuration:
                            result.ExecutionDuration = BitConverter.ToInt32(chunk.Item2, 0);
                            break;
                        case TokenErrorDetails:
                            result.Error = serializer.Deserialize(chunk.Item2, errorType) as Exception;
                            break;
                        case TokenErrorMessage:
                            result.ErrorMessage = Encoding.UTF8.GetString(chunk.Item2);
                            break;
                        case TokenErrorType:
                            result.ErrorType = Encoding.UTF8.GetString(chunk.Item2);
                            break;
                        case TokenStatus:
                            result.Status = (ProcessingStatus)chunk.Item2[0];
                            break;
                        case TokenEndOfCommand:
                            var t = Type.GetType(result.ContentType);
                            result.Content = serializer.Deserialize(content, t);
                            return result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Writes the message to stream.
        /// </summary>
        /// <param name="messageRecord">Message record.</param>
        public void Write(MessageRecord messageRecord)
        {
            var messageBytes = serializer.Serialize(messageRecord.Content);
            var errorBytes = messageRecord.Error != null ? serializer.Serialize(messageRecord.Error) : emptyBytes;
            var dataBytes = messageRecord.Data != null ? serializer.Serialize(messageRecord.Data) : emptyBytes;

            lock (objLock)
            {
                WriteChunk(TokenBeginOfCommand);
                WriteChunk(TokenId, messageRecord.Id.ToByteArray()); // id
                WriteChunk(TokenType, BitConverter.GetBytes(messageRecord.Type)); // type
                WriteChunk(TokenContentType, Encoding.UTF8.GetBytes(messageRecord.ContentType)); // message type
                WriteChunk(TokenCreated, BitConverter.GetBytes(messageRecord.CreatedAt.ToBinary())); // created
                WriteChunk(TokenExecutionDuration, BitConverter.GetBytes(messageRecord.ExecutionDuration)); // completed
                WriteChunk(TokenStatus, BitConverter.GetBytes((byte)messageRecord.Status)); // status
                if (messageRecord.Error != null)
                {
                    WriteChunk(TokenErrorDetails, errorBytes); // error
                }
                WriteChunk(TokenErrorMessage, Encoding.UTF8.GetBytes(messageRecord.ErrorMessage)); // error message
                WriteChunk(TokenErrorType, Encoding.UTF8.GetBytes(messageRecord.ErrorType)); // error type
                WriteChunk(TokenContent, messageBytes); // message object
                if (messageRecord.Data != null)
                {
                    WriteChunk(TokenData, dataBytes);
                }
                WriteChunk(TokenEndOfCommand);
            }
        }
    }
}
