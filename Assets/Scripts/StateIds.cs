using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Id {
    public static Id none;

    private readonly uint mId;

    public Id this[int childId] {
        get { return new Id((mId << 8) | (uint)childId); }
    }

    public Id super {
        get { return new Id(mId >> 8); }
    }

    public bool isa(Id super) {
        return (this != none) && ((this.super == super) || this.super.isa(super));
    }

    public static implicit operator Id(int id) {
        if (id == 0) {
            throw new System.InvalidCastException("Top level Id cannot be 0");
        }
        return new Id((uint)id);
    }

    public static bool operator ==(Id a, Id b) {
        return a.mId == b.mId;
    }

    public static bool operator !=(Id a, Id b) {
        return a.mId != b.mId;
    }

    public override bool Equals(object obj) {
        if (obj is Id) {
            return ((Id)obj).mId == mId;
        }
        else {
            return false;
        }
    }

    public override int GetHashCode() {
        return (int)mId;
    }

    private Id(uint id) {
        mId = id;
    }
}

[System.Serializable]
public static class StateId {

    public static class Player {
        public static readonly Id empty = 1;
        public static readonly Id moveMode = 2;
        public static class MoveModes {
            public static readonly Id grounded = moveMode[0];
            public static readonly Id air = moveMode[1];
            public static class Grounded {
                public static readonly Id sprint = grounded[0];
                public static readonly Id aiming = grounded[1];
                public static readonly Id freeRoam = grounded[2];
                public static readonly Id jump = grounded[3];
            }
            public static class Air {
                public static readonly Id falling = air[1];
                public static readonly Id land = air[2];
                public static readonly Id aiming = air[3];
            }
        }
    }

    public static class Camera {
        public static readonly Id grounded = 1;
        public static class Grounded {
            public static readonly Id aim = grounded[1];
            public static readonly Id freeRoam = grounded[2];
            public static readonly Id sprint = grounded[3];
        }
    }
}

