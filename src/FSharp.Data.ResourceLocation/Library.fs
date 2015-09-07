namespace FSharp.Data.ResourceLocation

open System
open ProviderImplementation.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices
open System.Reflection
open Arachne.Uri
open Arachne.Uri.Template

type Match =
  class
  end

module private Generator =
  let recVarSpec (VariableSpec(VariableName name, t)) = (name, t)
  let recVarList (Expression(_, VariableList xs)) = List.map recVarSpec xs

  let recExpr =
    function
    | UriTemplatePart.Expression x -> Some(recVarList x)
    | _ -> None

  let recTemplate (UriTemplate xs) =
    List.map recExpr xs
    |> List.choose id
    |> List.concat

  open ProviderImplementation.ProvidedTypes

  let clrType =
    function
    | None -> typeof<string>
    | _ -> typeof<string list>

  let matchProperty (name, t) = ProvidedProperty(name, clrType t)

  let matchedType template =
    let t = ProvidedTypeDefinition("Match", Some typeof<Match>)
    recTemplate template
    |> List.map matchProperty
    |> t.AddMembers
    t

  let matching (t : ProvidedTypeDefinition) template =
    let mt = matchedType template
    t.AddMember mt
    [ ProvidedMethod("match", [ ProvidedParameter("uri", typeof<string>) ], mt,IsStaticMethod = true)
      ProvidedMethod ("match", [ ProvidedParameter("uri", typeof<System.Uri>) ], mt,IsStaticMethod = true)]
    |> t.AddMembers

  let root (t : ProvidedTypeDefinition) template =
    match UriTemplate.TryParse template with
    | Some template ->
      t.AddXmlDoc(sprintf """
            %s
        """ (UriTemplate.Format template))
      matching t template
      t
    | None -> failwithf "Invalid uri template"

[<TypeProvider>]
type TeePee(config : TypeProviderConfig) as __ =
    inherit TypeProviderForNamespaces()
    do __.RegisterRuntimeAssemblyLocationAsProbingFolder config

    let ns = "FSharp.Data"
    let asm = Assembly.GetExecutingAssembly()
    let parameters = [ ProvidedStaticParameter("template", typeof<string>) ]

    let op = ProvidedTypeDefinition(asm, ns, "ResourceLocator", Some typeof<obj>)

    let create() =
        let init typeName (parameters : obj array) =
            let erased =
              ProvidedTypeDefinition(asm, ns, typeName, Some(typeof<obj>))

            match parameters with
                | [| :? string as template |] -> Generator.root erased template
                | e -> raise (ArgumentException(sprintf "Invalid parameters %A %A" parameters e))
        op.DefineStaticParameters(parameters, init)
        op

    do __.AddNamespace(ns, [ create() ])

[<assembly:TypeProviderAssembly>]
do ()
