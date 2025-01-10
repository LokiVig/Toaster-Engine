#pragma once

struct BBox
{
public:
    BBox() = default;
    BBox(vec3 mins, vec3 maxs) :
        mins(mins), maxs(maxs)
    {

    }

public:
    vec3 maxs;
    vec3 mins;
};