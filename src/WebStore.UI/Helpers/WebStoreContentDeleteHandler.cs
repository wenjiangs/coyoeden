﻿//  Author:                     Joe Audette
//  Created:                    2009-06-21
//	Last Modified:              2009-12-08
// 
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Text;
using Cynthia.Business;
using Cynthia.Business.WebHelpers;
using WebStore.Business;

namespace WebStore
{
    public class WebStoreContentDeleteHandler : ContentDeleteHandlerProvider
    {
        public WebStoreContentDeleteHandler()
        { }

        public override void DeleteContent(int moduleId, Guid moduleGuid)
        {
            Store.DeleteByModule(moduleId);
            ContentMetaRespository metaRepository = new ContentMetaRespository();
            metaRepository.DeleteByModule(moduleGuid);
            
        }
    }
}
