<?php
  require __DIR__ . '/../vendor/autoload.php';

  $dotenv = new Dotenv\Dotenv(__DIR__, "../_env");
  $dotenv->load();

  $token = getenv("WUNDERGROUND_API_TOKEN");
  $meteo = file_get_contents("http://api.wunderground.com/api/${token}/conditions/lang:IT/q/Italy/Desio.json");
  $json = json_decode($meteo, true);

  $condizione = $json['current_observation']['weather'];
  $temp= $json['current_observation']['temp_c'];
  $umidita=$json['current_observation']['relative_humidity'];
  $icon=$json['current_observation']['icon'];

  switch ($icon) {
    case "rain":
      $icont=":umbrella:";
      break;
  }

  echo 'Condizioni: '.$condizione." ".$icont."\r\n";
  echo 'Temperatura: '.$temp."° C\r\n";
  echo 'Umidità: '.$umidita."\r\n";
?>
