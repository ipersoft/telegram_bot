<?php
  $meteo = file_get_contents('http://api.wunderground.com/api/TOKEN/conditions/lang:IT/q/Italy/Desio.json');
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
