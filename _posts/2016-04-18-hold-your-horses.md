---
layout: post
title: Hold your horses
---

Quick one - I excluded from code coverage code decorated with CompileGenerated attribute. Wrong!

While for lambdas it sometimes may proove useful, it also elliminates code executed with _async_/_await_ keywords.
These generates state machine classes generated by the compiler which are also decorated with this attribute making my code dissapear from coverage.

I retracted that exclusion. Lucky that it didn't affect results to much.