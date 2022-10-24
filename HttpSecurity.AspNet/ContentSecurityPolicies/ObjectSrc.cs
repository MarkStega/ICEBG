﻿namespace HttpSecurity.AspNet;


/// <summary>
/// object-src policy.
/// </summary>
[ContentSecurityPolicyOptions]
[AddHashValue]
[AddHostSource]
[AddNone]
[AddNonce]
[AddReportSample]
[AddSelf]
[AddSchemeSource]
[AddStrictDynamic]
[AddUnsafeEval]
[AddUnsafeHashes]
[AddUnsafeInline]
[AddUri]
public sealed partial class ObjectSrcOptions : ContentSecurityPolicyOptionsBase
{
}


/// <summary>
/// object-src policy.
/// </summary>
[ContentSecurityPolicy("object-src")]
public sealed partial class ObjectSrc : ContentSecurityPolicyBase
{
}
