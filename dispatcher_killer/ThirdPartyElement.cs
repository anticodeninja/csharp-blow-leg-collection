// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed
// with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright 2018-2020 Artem Yamshanov, me [at] anticode.ninja

namespace DispatcherKiller
{
    using System;
    using System.Windows;

    public class ThirdPartyElement : UIElement
        {
            internal static Action Callback;

            protected override Size MeasureCore(Size availableSize)
            {
                Callback?.Invoke();
                return base.MeasureCore(availableSize);
            }
        }
}
