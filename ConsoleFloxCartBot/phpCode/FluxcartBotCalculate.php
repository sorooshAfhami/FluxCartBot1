<?php

require_once 'vendor/autoload.php';

use TelegramBot\Api\Client;
use TelegramBot\Api\Exception;
use TelegramBot\Api\Types\ReplyKeyboardMarkup;

// Initialize variables
$current = 0;
$total = 0;
$rate = 0;
$commission = 0;
$suggestCurrency = 0;
$suggestDay = 0;
$list = [];

// Initialize the bot
$bot = new Client('7218401198:AAGRcEI1y09uhV-CtwIsoeh4r9JLKLuVm5k');

try {
    // Handle /start command
    $bot->command('start', function ($message) use ($bot, &$current, &$total, &$rate, &$commission, &$suggestCurrency, &$list) {
        $current = 0;
        $total = 0;
        $rate = 0;
        $commission = 0;
        $suggestCurrency = 0;
        $list = [];

        $chatId = $message->getChat()->getId();
        $bot->sendMessage($chatId, "به ربات محاسبه درآمد فلوکارت خوش آمدید");
        $bot->sendMessage($chatId, "مقدار موجودی الان خود را وارد کنید و علامت $ در انتها قرار دهید");
    });

    // Handle incoming messages
    $bot->on(function ($message) use ($bot, &$current, &$total, &$rate, &$commission, &$suggestCurrency, &$list) {
        $chatId = $message->getChat()->getId();
        $text = $message->getText();

        try {
            if (strpos($text, '$') !== false) {
                $current = floatval(str_replace('$', '', $text));
                $bot->sendMessage($chatId, "درصد VIP خود را وارد کنید و علامت % را درانتها قرار دهید");
            }

            if (strpos($text, '%') !== false) {
                $rate = floatval(str_replace('%', '', $text)) / 100;
                $bot->sendMessage($chatId, "مقدار کمیسیون روزانه خود از تیم را وارد کنید و علامت & را در انتها وارد کنید");
            }

            if (strpos($text, '&') !== false) {
                $commission = floatval(str_replace('&', '', $text));
                $bot->sendMessage($chatId, "درآمد مورد انتظار خود را وارد کنید(به واحد تتر) و در انتها علامت # را وارد کنید");
            }

            if (strpos($text, '#') !== false) {
                $list = [];
                $suggestCurrency = floatval(str_replace('#', '', $text));
                $day = 0;
                $total = $current;

                while ($total < $suggestCurrency) {
                    for ($i = 1; $i <= 11; $i++) {
                        $total += $total * $rate;
                    }
                    $day++;
                    $total += $commission;
                    $list[] = "\n بعد از " . $day . " روز " . number_format($total, 2);
                }

                $bot->sendMessage($chatId, "\n" . $day . " روز نیاز است ");
                $bot->sendMessage($chatId, "درصورت نیاز به مشاهده درآمد روزهای محاسبه شده علامت + را وارد نمایید");
            }

            if ($text == '+') {
                if (count($list) > 60) {
                    $bot->sendMessage($chatId, "تعداد روز بیشتر از 60 است و فرآیند طولانی می شود لذا جزییات نمایش داده نمی شود.");
                    $bot->sendMessage($chatId, "احتمالا درآمد مورد انتظار خود را زیادتر از حد معمول وارد کرده اید");
                } else {
                    $bot->sendMessage($chatId, "به تعداد روزهای نمایش داده شده در پیام قبلی باید رکورد نمایش داده شود. لذا کمی صبور باشد");
                    foreach ($list as $item) {
                        $bot->sendMessage($chatId, $item);
                    }
                }
            }
        } catch (Exception $ex) {
            // Handle exceptions
        }
    }, function () {
        return true;
    });

    // Start polling the bot
    $bot->run();

} catch (Exception $e) {
    echo 'Error: ' . $e->getMessage();
}
