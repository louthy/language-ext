namespace LanguageExt

open System
open ProcessFs

module RoleFs =

    let Broadcast  = LanguageExt.Role.Broadcast
    let LeastBusy  = LanguageExt.Role.LeastBusy
    let RoundRobin = LanguageExt.Role.RoundRobin
    let Random     = LanguageExt.Role.Random
