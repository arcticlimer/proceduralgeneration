{
  description = "Flake utils demo";

  inputs.flake-utils.url = "github:numtide/flake-utils";

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (
      system:
        let
          pkgs = import nixpkgs {
            inherit system;
            config = { allowUnfree = true; };
          };
        in
          rec {
            devShell = pkgs.mkShell {
              buildInputs = with pkgs; [ pythonFull aseprite-unfree unityhub ];
            };
          }
    );
}
