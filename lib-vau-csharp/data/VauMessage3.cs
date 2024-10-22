﻿/*
 * Copyright 2024 gematik GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using lib_vau_csharp.util;
using Newtonsoft.Json;
using PeterO.Cbor;
using System;

namespace lib_vau_csharp.data
{
    public class VauMessage3
    {
        private const string _messageType = "M3";
        [JsonProperty("MessageType")]
        public string MessageType => _messageType;
        [JsonProperty("AEAD_ct")]
        public byte[] AeadCt { get; private set; }
        [JsonProperty("AEAD_ct_key_confirmation")]
        public byte[] AeadCtKeyKonfirmation { get; private set; }

        [JsonConstructor]
        public VauMessage3(byte[] aeadCt, byte[] aeadCtKeyKonfirmation)
        {
            AeadCt = aeadCt;
            AeadCtKeyKonfirmation = aeadCtKeyKonfirmation;
        }

        public byte[] toCbor() {
          CBORObject cborVauKey = CBORObject.NewOrderedMap();     // Reihenfolge Tags wird beibehalten (prinzipiell ist die Reihenfolge egal)
          cborVauKey.Add("MessageType", _messageType);
          cborVauKey.Add("AEAD_ct", AeadCt);
          cborVauKey.Add("AEAD_ct_key_confirmation", AeadCtKeyKonfirmation);

          return cborVauKey.EncodeToBytes();
        }

        public static VauMessage3 fromCbor(byte[] encodedObject)
        {
            try
            {
                CBORObject cborObject = CBORObject.DecodeFromBytes(encodedObject);
                if (cborObject["MessageType"].AsString() != _messageType)
                {
                    throw new CBORException("This CBor object is not a VauMessage3 object!");
                }

                VauMessage3 vauMessage3 = new VauMessage3(
                    CborUtils.DecodeByteValueFromCbor(cborObject["AEAD_ct"]),
                    CborUtils.DecodeByteValueFromCbor(cborObject["AEAD_ct_key_confirmation"]));
                return vauMessage3;
            }
            catch (Exception e)
            {
                throw new CBORException("Error when decoding CBOR to VauMessage3.", e.InnerException);
            }
        }
    }
}
