#pragma once

struct BBox
{
public:
    BBox() = default;

public:
    vec3 maxs;
    vec3 mins;
};